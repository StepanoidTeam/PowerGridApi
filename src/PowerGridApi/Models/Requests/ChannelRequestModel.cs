
using System;

namespace PowerGridEngine
{
    public class ChannelRequestModel : IWebSocketRequestModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
