
using System;

namespace PowerGridEngine
{
    public class ChatSendModel : IWebSocketRequestModel
    {
        public string ChannelId { get; set; }

        public string Message { get; set; }

    }
}
