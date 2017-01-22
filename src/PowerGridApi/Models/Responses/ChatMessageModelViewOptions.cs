
using PowerGridEngine;

namespace PowerGridApi
{ 
    public class ChatMessageModelViewOptions : AbstractModelViewOptions
    {
        public bool ChannelId { get; set; }
        
        public bool Date { get; set; }
      
        public bool Message { get; set; }
       
        public bool SenderId { get; set; }

        public bool SenderName { get; set; }
        
        private void Init(bool defaultValue = false)
        {
            ChannelId = defaultValue;
            Date = defaultValue;
            Message = defaultValue;
            SenderId = defaultValue;
            SenderName = defaultValue;
        }

        public ChatMessageModelViewOptions()
        {
            Init();
        }

        public ChatMessageModelViewOptions(bool defaultValue)
        {
            Init(defaultValue);
        }
    }
}
