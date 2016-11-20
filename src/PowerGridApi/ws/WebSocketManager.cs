using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PowerGridApi.ws
{
	//todo: make it singletone or IoC/DI
	public static class WebSocketManager
	{
		static ConcurrentBag<WebSocket> _sockets = new ConcurrentBag<WebSocket>();



		//todo: specify sender/receivers? not for all
		public static async void Broadcast(string message)
		{
			await Task.WhenAll(
				_sockets
				.Where(s => s.State == WebSocketState.Open)
				.Select(s => s.SendAsync(message.GetByteSegment(), WebSocketMessageType.Text, true, CancellationToken.None)));
		}


		public static async Task Listen(HttpContext http, Func<Task> next)
		{
			//_sockets.Add(webSocket);
			if (http.WebSockets.IsWebSocketRequest)
			{
				var webSocket = await http.WebSockets.AcceptWebSocketAsync();


				if (webSocket != null && webSocket.State == WebSocketState.Open)
				{
					_sockets.Add(webSocket);


					//todo: move this block to the separate method
					while (webSocket.State == WebSocketState.Open)
					{
						try
						{
							var buffer = new ArraySegment<byte>(new byte[4096]);
							var received = await webSocket.ReceiveAsync(buffer, CancellationToken.None);


							//todo: handle only text?
							switch (received.MessageType)
							{
								case WebSocketMessageType.Close:
									//todo: somehow delete closed sockets
									OnClose(received.CloseStatus, received.CloseStatusDescription);
									break;
								case WebSocketMessageType.Text:
									var message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0');

									//todo: somehow parse json to model?
									//todo: route the requests to proper methods?
									OnMessage(message);
									Broadcast(message);
									break;
								case WebSocketMessageType.Binary:
									OnMessage(buffer.Array);
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
		}


		static void OnClose(WebSocketCloseStatus? status, string desc)
		{
			//todo: somehow delete closed sockets
			Console.WriteLine(desc);
		}

		static void OnMessage(string data)
		{
			//todo: somehow parse json to model?
			//todo: route the requests to proper methods?
			Console.WriteLine(data);
		}

		static void OnMessage(byte[] data)
		{
			Console.WriteLine(data);
		}

	}
}
