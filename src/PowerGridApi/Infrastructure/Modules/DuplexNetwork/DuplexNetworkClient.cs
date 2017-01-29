using PowerGridEngine;
using System;
using System.Net.WebSockets;

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
            LastActivityTime = DateTime.UtcNow;
        }
    }
}
