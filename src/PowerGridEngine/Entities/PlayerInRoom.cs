
namespace PowerGridEngine
{
    /// <summary>
    /// Sense of this entity is different with User entity. This one should allow to completely change User
    /// (in case one User would to leave or even leave, without player game could not be played and players invite
    /// another user to join to game Instead of current player with all current player achievements)
    /// </summary>
    public class PlayerInRoom
    {
        public User Player { get; set; }

        public PlayerInRoom(User player)
        {
            Player = player;
        }
    }
}
