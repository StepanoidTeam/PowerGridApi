using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PowerGridEngine;
using System.Collections.Generic;

namespace PowerGridApi
{
    public class WebSocketManager
    {
        private static WebSocketManager _current;

        private ConcurrentBag<DuplexNetworkClient> _clients { get; set; }

        public delegate void RequestRecievedDelegate(User user, DuplexNetworkRequestType type, string json);
        public delegate void ConnectionCloseDelegate(User user);

        public event ConnectionCloseDelegate OnClose;
        public event RequestRecievedDelegate OnMessage;

        //todo: make it IoC/DI?
        public static WebSocketManager Current
        {
            get
            {
                if (_current == null)
                    _current = new WebSocketManager();
                return _current;
            }
        }

        public WebSocketManager()
        {
            _clients = new ConcurrentBag<DuplexNetworkClient>();
            OnClose += WebSocketManager_onClose;
            OnMessage += WebSocketManager_onMessage;
        }

        private void CloseUnactiveConnections(WebSocketState[] statuses = null)
        {
            statuses = statuses ?? new[] { WebSocketState.Aborted, WebSocketState.Closed };

            var closedClients = _clients.Where(m => statuses.Contains(m.Connection.State)).ToArray();
            var closedUsers = closedClients.Where(m => m.User != null).Select(m => m.User).ToList();

            _clients.RemoveItems(closedClients);
            closedUsers.ForEach(m => OnClose(m));
        }

        /// <summary>
        /// receiversId could be roomId, userId or null (global broadcast)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="receiversId"></param>
        public async void Broadcast<T>(T response, string receiverId = null)
        {
            CloseUnactiveConnections();

            var message = response.ToJson();
            var data = message.GetByteSegment();

            var receivers = _clients.Where(s => s.Connection.State == WebSocketState.Open);
            receivers = receivers.Where(s => s.User != null);

            var room = EnergoServer.Current.TryToLookupRoom(receiverId);
            if (room != null)
            {
                receivers = receivers.Where(m => m.User.IsInRoom(room.Id));
            }
            else
            {
                var receiver = EnergoServer.Current.TryToLookupUser(receiverId);
                if (receiver != null)
                    receivers = receivers.Where(m => m.User.Id == receiver.Id);
            }

            await Task.WhenAll(receivers.Select(s => s.Connection.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None)));
        }

        public async void Broadcast<T>(T response, List<string> receiverIds)
        {
            CloseUnactiveConnections();

            var message = response.ToJson();
            var data = message.GetByteSegment();

            var receivers = _clients.Where(s => s.Connection.State == WebSocketState.Open);
            receivers = receivers.Where(s => receiverIds.Contains(s.User.Id));

            await Task.WhenAll(receivers.Select(s => s.Connection.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None)));
        }

        private bool CheckAuthorization(DuplexNetworkClient client, DuplexNetworkRequest request)
        {
            var errMsg = string.Empty;
            if (client.User == null) //try to authorize if not yet
                client.User = EnergoServer.Current.FindUserByAuthToken(request.AuthToken, out errMsg);

            return client.User != null;
        }

        /// <summary>
        /// Unassign logged out user from Socket Connection. Needed to logout user (unattach it from connection)
        /// </summary>
        public void ForgotUser(User user)
        {
            var client = _clients.FirstOrDefault(m => m.User != null && m.User.Id == user.Id);
            if (client != null)
                client.User = null;
        }

        /// <summary>
        /// It will expecting to recieve AuthToken as Json parameter from client to join it to network
        /// </summary>
        /// <param name="http"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task HandleRequests(HttpContext http, Func<Task> next)
        {
            //_sockets.Add(webSocket);
            if (http.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await http.WebSockets.AcceptWebSocketAsync();

                if (webSocket != null && webSocket.State == WebSocketState.Open)
                {
                    CloseUnactiveConnections();

                    var client = new DuplexNetworkClient { Connection = webSocket };
                    _clients.Add(client);

                    //todo: move this block to the separate method
                    while (webSocket.State == WebSocketState.Open)
                    {
                        try
                        {
                            //todo buffer size means it will trunc or separate requests if they will exceed 4k? Need to handle this case?
                            var buffer = new ArraySegment<byte>(new byte[4096]);
                            var received = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                            //todo: handle only text?
                            switch (received.MessageType)
                            {
                                case WebSocketMessageType.Close:
                                    CloseUnactiveConnections(new[] { WebSocketState.CloseReceived });
                                    break;

                                case WebSocketMessageType.Text:
                                    var message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0');
                                    var request = message.ToObject<DuplexNetworkRequest>();

                                    if (CheckAuthorization(client, request))
                                        OnMessage(client.User, request.Type, message);
                                    else
                                        ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, "Websocket Unauthorized client trying to send something");
                                    break;

                                case WebSocketMessageType.Binary:
                                    //todo: not impl yet for binary - do we need this case?
                                    ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, "Websocket Binary recieved:");
                                    ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, buffer.Array);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            ServerContext.Current.Logger.LogError(LogDestination.Console, ex);
                            break;
                        }
                    }
                    //todo: handle closed socket - remove?
                }
                else
                {
                    // Nothing to do here, pass downstream.  
                    await next();
                }
            }
            else
                await next();
        }

        private void WebSocketManager_onClose(User user)
        {
            string userId = null;
            string userName = null;
            if (user != null)
            {
                userId = user.Id;
                userName = user.Username;
            }
            ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, "Websocket onClose from Id = {0}, name = {1}.", userId, userName);
        }

        private void WebSocketManager_onMessage(User user, DuplexNetworkRequestType type, string json)
        {
            string userId = null;
            string userName = null;
            if (user != null)
            {
                userId = user.Id;
                userName = user.Username;
            }
            ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, "Websocket onMessage from Id = {0}, name = {1}: {2}", userId, userName, json);
        }
    }
}
