using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
	public class Player : BaseEnergoEntity
	{
		public override BaseEnergoViewModel ToViewModel(IViewModelOptions options = null)
		{
			var ret = new PlayerViewModel();
			var opts = new PlayerViewModelOptions(true);
			if (options != null)
				opts = options as PlayerViewModelOptions;
			if (opts.Id)
				ret.UserId = this.Id;
			if (opts.Name)
				ret.Username = this.Username;
			if (opts.GameRoomId)
				ret.GameRoomId = this.GameRoomRef == null ? null : this.GameRoomRef.Id;
			if (opts.ReadyMark)
				ret.ReadyMark = this.GameRoomRef == null ? false : this.GameRoomRef.Players[this.Id].ReadyMark;
			return ret;
		}

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
