
namespace PowerGridEngine
{
	public class Player : BaseEnergoEntity
	{
		public string Id { get; private set; }

		public string Username { get; private set; }

		public GameRoom GameRoomRef { get; set; }

		public Player(string username, string id = null)
		{
			if (string.IsNullOrWhiteSpace(id))
				Id = EnergoServer.MakeId(username);
			else
				Id = id;
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
