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
    [Flags]
    public enum SubscriberType
    {
        None = 0,
        User = 1,
        Room = 2,
        Channel = 4
    }

    public class WebSocketManager
    {
        private static WebSocketManager _current;

        private ConcurrentBag<DuplexNetworkClient> _clients { get; set; }

        public delegate void RequestRecievedDelegate(User user, DuplexNetworkRequestType type, string json);
        public delegate void ConnectionCloseDelegate(User user);

        public event ConnectionCloseDelegate OnClose;
        public event RequestRecievedDelegate OnRequestRecieved;

        public WebSocketManager()
        {
            _clients = new ConcurrentBag<DuplexNetworkClient>();
            OnClose += WebSocketManager_OnClose;
            OnRequestRecieved += WebSocketManager_OnRequestRecieved;
        }

        /// <summary>
        /// Currently it's just a trick, need to find more intelegent way to close offline (for websocket perspective) users.
        /// It will log out user and remove from connections if he disconnected from socked and wasn't send messages into socket for some time
        /// </summary>
        /// <param name="statuses"></param>
        private void CloseUnactiveConnections(WebSocketState[] statuses = null)
        {
            statuses = statuses ?? new[] { WebSocketState.Aborted, WebSocketState.Closed };

            var closedClients = _clients.Where(m => statuses.Contains(m.Connection.State)).ToArray();

            //will close users without activity for last 1 hour
            closedClients = closedClients.Where(m => m.LastActivityTime.AddHours(1) > DateTime.UtcNow).ToArray();

            var closedUsers = closedClients.Where(m => m.User != null).Select(m => m.User).ToList();

            _clients.RemoveItems(closedClients);
            closedUsers.ForEach(m =>
            {
                OnClose(m);
                ServerContext.Current.UserModule.Logout(m);
            });
        }

        /// <summary>
        /// receiversId could be roomId, userId or null (global broadcast)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="receiverId"></param>
        /// <param name="receiverType"></param>
        public async void Broadcast<T>(T response, string receiverId = null, SubscriberType receiverType = SubscriberType.None)
        {
            await Broadcast(response, receiverId == null ? null : new List<string> { receiverId }, receiverType);
        }

        /// <summary>
        /// ReceiverIds should be null or empty for case you want to wide broadcast for ALL online users
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="receiverIds"></param>
        /// <param name="receiverType">Ignored in case ReceiverIds is null or empty</param>
        public async Task Broadcast<T>(T response, List<string> receiverIds, SubscriberType receiverType = SubscriberType.None)
        {
            receiverIds = receiverIds ?? new List<string>();

            CloseUnactiveConnections();

            var message = response.ToJson();
            var data = message.GetByteSegment();

            var receivers = _clients.Where(s => s.Connection.State == WebSocketState.Open);
            receivers = receivers.Where(s => s.User != null);

            var userReceiverIds = new List<string>();
            foreach (var receiverId in receiverIds)
            {
                var userReceivers = new List<string>();
                if (receiverType.HasFlag(SubscriberType.User))
                {
                    var result = EnergoServer.Current.TryToLookupUser(receiverId);
                    if (result != null)
                        userReceivers.Add(result.Id);
                }
                if (receiverType.HasFlag(SubscriberType.Room) && !userReceivers.Any())
                {
                    var room = EnergoServer.Current.TryToLookupRoom(receiverId);
                    if (room != null)
                    {
                        userReceivers = room.Players.Values.Select(m => m.Player.Id).ToList();
                    }
                }
                if (receiverType.HasFlag(SubscriberType.Channel) && !userReceivers.Any())
                {
                    var channel = ServerContext.Current.Chat.LookupChannel(null, receiverId);
                    if (channel != null)
                    {
                        //for private it's not possible to get correct list of subscribers, because there are all
                        //users ever sent messages in private
                        if (channel.Type == ChatChannelType.Private)
                            userReceivers.Add(channel.Id);
                        else
                            userReceivers = channel.Subscribers.Keys.ToList();
                    }
                }
                if (userReceivers.Any())
                    userReceiverIds.AddRange(userReceivers);
            }
            if (userReceiverIds.Any())
                receivers = receivers.Where(s => userReceiverIds.Contains(s.User.Id));

            await Task.WhenAll(receivers.Select(s => s.Connection.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None)));
        }

        private async Task DirectResponse<T>(DuplexNetworkClient client, T response)
        {
            var message = response.ToJson();
            var data = message.GetByteSegment();

            if (client.Connection.State == WebSocketState.Open)
                await client.Connection.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// It will also put user into status 'Online' (means it's available to get messages via socket) if authorized
        /// and also updated last connection activity (for destroy connections it they was not active some time)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool CheckAuthorization(DuplexNetworkClient client, DuplexNetworkRequest request)
        {
            var errMsg = string.Empty;
            if (client.User == null) //try to authorize if not yet
                client.User = EnergoServer.Current.FindUserByAuthToken(request.AuthToken, out errMsg);

            return client.User != null;
        }

        /// <summary>
        /// Unassign logged user from Socket Connection in case connection closed (it means user is not logged out, but is offline).
        /// </summary>
        public void SetUserAsOffline(User user)
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
            if (!http.WebSockets.IsWebSocketRequest)
            {
                await next();
                return;
            }

            var webSocket = await http.WebSockets.AcceptWebSocketAsync();

            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                CloseUnactiveConnections();

                var client = new DuplexNetworkClient(webSocket);
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
                                    OnRequestRecieved(client.User, request.Type, message);
                                else
                                {
                                    var response = new { IsSuccess = false, Message = "You are not authorized" };
                                    await DirectResponse(client, response);
                                    ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, "Websocket Unauthorized client trying to send something");
                                }
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

        private void WebSocketManager_OnClose(User user)
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

        private void WebSocketManager_OnRequestRecieved(User user, DuplexNetworkRequestType type, string json)
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
