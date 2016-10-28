
namespace PowerGridEngine
{
	public class User : BaseEnergoEntity
	{
        /// <summary>
        /// For use user by another user (like get other user info or invite another user into room)
        /// </summary>
        public string PublicId { get; private set; }

        /// <summary>
        /// Authorization token of current user to check if he has permission for some action
        /// </summary>
		public string Id { get; private set; }

		public string Username { get; private set; }

		public GameRoom GameRoomRef { get; set; }

		public User(string username, string id = null)
		{
			if (string.IsNullOrWhiteSpace(id))
				Id = EnergoServer.MakeId(username);
			else
				Id = id;
            PublicId = EnergoServer.MakeId();
            Username = username.RemoveExtraSpaces();
		}

		/// <summary>
		/// is in not started game room
		/// </summary>
		/// <returns></returns>
		public bool IsInRoom()
		{
			return GameRoomRef != null;
		}

		/// <summary>
		/// is in started game
		/// </summary>
		/// <returns></returns>
		public bool IsInGame()
		{
			return GameRoomRef != null && GameRoomRef.GameBoardRef != null;
		}
	}
}
