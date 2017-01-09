
using System;

namespace PowerGridEngine
{
    public class InviteToChannelModel : IWebSocketRequestModel
    {
        public string ChannelId { get; set; }

        public string UserId { get; set; }
    }
}
