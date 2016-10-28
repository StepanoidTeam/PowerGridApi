
namespace PowerGridEngine
{
    public class PlayerInRoom
    {
        public User Player { get; set; }
        public bool ReadyMark { get; set; }

        public PlayerInRoom(User player)
        {
            Player = player;
        }
    }
}
