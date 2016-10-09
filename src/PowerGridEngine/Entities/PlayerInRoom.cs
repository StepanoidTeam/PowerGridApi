
namespace PowerGridEngine
{
    public class PlayerInRoom
    {
        public Player Player { get; set; }
        public bool ReadyMark { get; set; }

        public PlayerInRoom(Player player)
        {
            Player = player;
        }
    }
}
