
namespace PowerGridEngine
{  
    public class UserModelViewOptions : AbstractIdNameModelViewOptions
    {   
        public bool GameRoomId { get; set; }
       
        public bool ReadyMark { get; set; }

        public UserModelViewOptions(bool defaultValue = false) : base(defaultValue)
        {
            GameRoomId = defaultValue;
            ReadyMark = defaultValue;
        }
    }
}
