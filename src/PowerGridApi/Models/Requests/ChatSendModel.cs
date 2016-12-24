
using System;

namespace PowerGridEngine
{
    public class ChatSendModel : IWebSocketRequestModel
    {
        public string To { get; set; }

        public bool InRoomChannel { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public string SenderId { get; set; }

        public string SenderName { get; set; }
    }
}
