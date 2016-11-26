using PowerGridEngine;
using System;

namespace PowerGridApi
{
    public class WebSocketResponse
    {
        public string UserId { get; set; }

        public string Data { get; set; }

        public NetworkChannel Channel { get; set; }
    }
}
