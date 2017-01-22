
using System;

namespace PowerGridEngine
{
    public class GetChatMessagesRequestModel : IWebSocketRequestModel
    {
        public string Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
