
namespace PowerGridEngine
{
	public class User : BaseEnergoEntity
	{
        /// <summary>
        ///  Authorization token of current user to check if he has permission for some action
        /// </summary>
        public string AuthToken { get; private set; }

        /// <summary>
        /// For use user by another user (like get other user info or invite another user into room)
        /// </summary>
		public string Id { get; private set; }

		public string Username { get; private set; }

		public GameRoom GameRoomRef { get; set; }

		public User(string username, string id = null)
		{
			if (string.IsNullOrWhiteSpace(id))
				Id = username.NormalizeId();
			else
				Id = id;
            if (Id == EnergoServer.AdminId)
                AuthToken = EnergoServer.AdminAuthToken;
            else
                AuthToken = EnergoServer.GenerateId();
            Username = username.RemoveExtraSpaces();
		}

		/// <summary>
		/// is in not started game room
		/// </summary>
		/// <returns></returns>
		public bool IsInRoom(string id = null)
		{
            return GameRoomRef != null && (id == null || id == GameRoomRef.Id);
        }

		/// <summary>
		/// is in started game
		/// </summary>
		/// <returns></returns>
		public bool IsInGame()
		{
            return GameRoomRef != null && GameRoomRef.IsInGame;
		}
	}
}
