using PowerGridEngine;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridApi
{
    public class ChatMessageModel : BaseEnergoModel<ChatMessage, ChatMessageModelViewOptions>
    {
        public string ChannelId { get { return Entity.ChannelId; } }

        public string Date { get { return Entity.Date.ToString("yyyy-MM-ddTHH:mm:ss"); } }

        public string Message { get { return Entity.Message; } }

        public string SenderId { get { return Entity.SenderId; } }

        public string SenderName { get { return Entity.SenderName; } }

        public ChatMessageModel(ChatMessage entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(ChatMessageModelViewOptions options = null)
        {
            if (options == null)
                options = new ChatMessageModelViewOptions(false);

            var result = new Dictionary<string, object>();
            if (options.ChannelId)
                result.Add("ChannelId", this.ChannelId);
            if (options.Date)
                result.Add("Date", this.Date);
            if (options.Message)
                result.Add("Message", this.Message);
            if (options.SenderId)
                result.Add("SenderId", this.SenderId);
            if (options.SenderName)
                result.Add("SenderName", this.SenderName);

            return result;
        }
    }
}
