
namespace PowerGridEngine
{ 
    public class RoomModelViewOptions : AbstractModelViewOptions
    {
        public bool Id { get; set; }
        
        public bool Name { get; set; }
      
        public bool IsInGame { get; set; }
       
        public bool UserCount { get; set; }
        
        public bool UserDetails { get; set; }
        
        public UserModelViewOptions UserViewOptions { get; set; }

        private void Init(bool defaultValue = false, UserModelViewOptions userViewOptions = null)
        {
            Id = defaultValue;
            Name = defaultValue;
            IsInGame = defaultValue;
            UserCount = defaultValue;
            UserDetails = defaultValue;
            UserViewOptions = userViewOptions ?? new UserModelViewOptions(defaultValue);
        }

        public RoomModelViewOptions()
        {
            Init();
        }

        public RoomModelViewOptions(bool defaultValue)
        {
            Init(defaultValue);
        }

        public RoomModelViewOptions(UserModelViewOptions userViewOptions)
        {
            Init(userViewOptions: userViewOptions);
        }
    }
}
