using PowerGridEngine;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PowerGridApi
{
    public class DuplexNetworkClient
    {
        public User User { get; set; }

        public WebSocket Connection { get; set; }

        public DateTime LastActivityTime { get; set; }

        public DuplexNetworkClient(WebSocket connection)
        {
            Connection = connection;
            UpdateActivity();
        }

        public void UpdateActivity()
        {
            LastActivityTime = DateTime.UtcNow;
        }

        public async Task SendData(ArraySegment<byte> data)
        {
            UpdateActivity();
            await Connection.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
