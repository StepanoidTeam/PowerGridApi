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
	public static class WebSocketManager
	{

		static ConcurrentBag<WebSocket> _sockets = new ConcurrentBag<WebSocket>();

		static ArraySegment<Byte> GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return new ArraySegment<Byte>(bytes);
		}

		public static async void Broadcast(string message)
		{
			//await Task.WhenAll(_sockets.Where(s.State == WebSocketState.Open).Select(s => s.Send());

			var webSocket = _sockets.First();

			await Task.WhenAll(
				_sockets
				.Where(s => s.State == WebSocketState.Open)
				.Select(s => s.SendAsync(GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None)));

			//await webSocket.SendAsync(GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
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
					//todo: somehow delete closed sockets

					while (webSocket.State == WebSocketState.Open)
					{
						var error = String.Empty;
						try
						{
							var buffer = new ArraySegment<Byte>(new Byte[4096]);
							var received = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

							switch (received.MessageType)
							{
								case WebSocketMessageType.Close:
									OnClose(received.CloseStatus, received.CloseStatusDescription);
									//todo: somehow delete closed sockets
									break;
								case WebSocketMessageType.Text:
									var message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0');
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
			Console.WriteLine(desc);
		}

		static void OnMessage(string data)
		{
			Console.WriteLine(data);
		}

		static void OnMessage(byte[] data)
		{
			Console.WriteLine(data);
		}

	}
}
