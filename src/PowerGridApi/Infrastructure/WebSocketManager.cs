using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PowerGridEngine;

namespace PowerGridApi
{
	public class WebSocketManager
	{
		public delegate void NetworkDelegate(string authToken, string message);

		private static WebSocketManager _current;

		private ConcurrentBag<WebSocketClient> _clients { get; set; }

		public event NetworkDelegate OnClose;
		public event NetworkDelegate OnMessage;

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
			_clients = new ConcurrentBag<WebSocketClient>();
			OnClose += WebSocketManager_onClose;
			OnMessage += WebSocketManager_onMessage;
		}

		/// <summary>
		/// receiversId could be roomId, userId or null (global broadcast)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="response"></param>
		/// <param name="receiversId"></param>
		public async void Broadcast<T>(T response, string receiversId = null)
		{
			var message = response.ToJson();
			var data = message.GetByteSegment();

			var receivers = _clients.Where(s => s.Socket.State == WebSocketState.Open && s.User != null);

			var room = EnergoServer.Current.TryToLookupRoom(receiversId);
			if (room != null)
			{
				receivers = receivers.Where(m => m.User.IsInRoom(room.Id));
			}
			else
			{
				var receiver = EnergoServer.Current.TryToLookupRoom(receiversId);
				if (receiver != null)
					receivers = receivers.Where(m => m.User.Id == receiver.Id);
			}

			await Task.WhenAll(receivers.Select(s => s.Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None)));
		}

		private bool CheckAuthorization(WebSocketClient client, WebSocketRequest request)
		{
			var errMsg = string.Empty;
			if (client.User == null) //try to authorize if not yet
				client.User = EnergoServer.Current.FindUserByAuthToken(request.AuthToken, out errMsg);

			return client.User != null;
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
					var client = new WebSocketClient { Socket = webSocket };
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
									//todo: somehow delete closed sockets
									//received.CloseStatus, 

									//todo: how we will indentify auth token here... ?
									OnClose(null, received.CloseStatusDescription);
									break;
								case WebSocketMessageType.Text:
									var message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0');
									var request = message.ToObject<WebSocketRequest>();

									if (CheckAuthorization(client, request))
										OnMessage(request.AuthToken, request.Message);
									else
										Console.WriteLine("Unauthorized client trying to send something");

									break;
								case WebSocketMessageType.Binary:
									//todo: not impl yet for binary - do we need this case?
									Console.WriteLine("binary:");
									Console.WriteLine(buffer.Array);
									break;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex);
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

		private void WebSocketManager_onClose(string authToken, string message)
		{
			Console.WriteLine("onClose:");
			Console.WriteLine(message);
		}

		private void WebSocketManager_onMessage(string authToken, string message)
		{
			//Broadcast(authToken, message);
			//todo: somehow parse json to model?
			//todo: route the requests to proper methods?
			Console.WriteLine("onMessage:");
			Console.WriteLine(message);
		}

	}
}
