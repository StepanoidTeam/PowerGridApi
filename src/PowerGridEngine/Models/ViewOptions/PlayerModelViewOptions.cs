
namespace PowerGridEngine
{  
    public class PlayerModelViewOptions : AbstractIdNameModelViewOptions
    {   
        public bool GameRoomId { get; set; }
       
        public bool ReadyMark { get; set; }

        public PlayerModelViewOptions(bool defaultValue = false) : base(defaultValue)
        {
            GameRoomId = defaultValue;
            ReadyMark = defaultValue;
        }
    }
}
