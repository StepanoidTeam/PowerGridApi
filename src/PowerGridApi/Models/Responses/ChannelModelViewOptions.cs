
using PowerGridEngine;

namespace PowerGridApi
{ 
    public class ChannelModelViewOptions : AbstractModelViewOptions
    {
        public bool Id { get; set; }
        
        public bool Name { get; set; }
      
        public bool Type { get; set; }
       
        public bool IsJoinedOrInvited { get; set; }

        private void Init(bool defaultValue = false)
        {
            Id = defaultValue;
            Name = defaultValue;
            Type = defaultValue;
            IsJoinedOrInvited = defaultValue;
        }

        public ChannelModelViewOptions()
        {
            Init();
        }

        public ChannelModelViewOptions(bool defaultValue)
        {
            Init(defaultValue);
        }
    }
}
