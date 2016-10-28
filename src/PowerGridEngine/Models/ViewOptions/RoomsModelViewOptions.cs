
namespace PowerGridEngine
{ 
    public class RoomModelViewOptions : AbstractModelViewOptions
    {
        public bool Id { get; set; }
        
        public bool Name { get; set; }
      
        public bool IsInGame { get; set; }
       
        public bool PlayerCount { get; set; }

        /// <summary>
        /// Id + Name
        /// </summary>
        
        public bool PlayerHeaders { get; set; }
              
        public bool PlayerDetails { get; set; }
        
        public UserModelViewOptions PlayerViewOptions { get; set; }

        private void Init(bool defaultValue = false)
        {
            Id = defaultValue;
            Name = defaultValue;
            IsInGame = defaultValue;
            PlayerCount = defaultValue;
            PlayerHeaders = defaultValue;
            PlayerDetails = defaultValue;
            PlayerViewOptions = new UserModelViewOptions(defaultValue);
        }

        public RoomModelViewOptions()
        {
            Init();
        }

        public RoomModelViewOptions(bool defaultValue)
        {
            Init(defaultValue);
        }
    }
}
