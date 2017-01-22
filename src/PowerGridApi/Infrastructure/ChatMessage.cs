
using System;

namespace PowerGridEngine
{
    public class ChatMessage: BaseEnergoEntity
    {
        public string ChannelId { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public string SenderId { get; set; }

        /// <summary>
        /// deprecated
        /// </summary>
        public string SenderName { get; set; }
    }
}
