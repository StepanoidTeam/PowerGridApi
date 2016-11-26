using PowerGridEngine;
using System;
using System.Net.WebSockets;

namespace PowerGridApi
{
    public class WebSocketClient
    {
        public User User { get; set; }

        public WebSocket Socket { get; set; }
    }
}
