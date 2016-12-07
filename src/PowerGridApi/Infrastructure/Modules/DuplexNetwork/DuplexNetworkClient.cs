using PowerGridEngine;
using System;
using System.Net.WebSockets;

namespace PowerGridApi
{
    public class DuplexNetworkClient
    {
        public User User { get; set; }

        public WebSocket Connection { get; set; }
    }
}
