using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
	public class EnergoServer
	{
        private static EnergoServer _current;

        /// <summary>
        /// You can use this singleton instance or create new one (and store it somewhere else), as you wish
        /// </summary>
        public static EnergoServer Current
        {
            get
            {
                if (_current == null)
                    _current = new EnergoServer();
                return _current;
            }
        }

        public static string MakeId(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return string.Empty;
			return str.ToLowerInvariant().Trim().Replace(" ", "");
		}

		public static string MakeId()
		{
			return Guid.NewGuid().ToString();
		}

		private IDictionary<string, Player> Players { get; set; }
		private IDictionary<string, Map> Maps { get; set; }
		private IDictionary<string, GameRoom> GameRooms { get; set; }
		public ServerSettings Settings { get; set; }

		public EnergoServer(ServerSettings settings = null)
		{
			Settings = new ServerSettings();
			if (settings != null)
				Settings = settings;
			Players = new Dictionary<string, Player>();
			Maps = new Dictionary<string, Map>();
			GameRooms = new Dictionary<string, GameRoom>();
            var mapCreator = new DefaultMapCreator();
			RegisterMap(mapCreator.Map);
		}

		//PLAYERS:

		public string Login(string username, out string errMsg, string userId = null)
		{
			errMsg = string.Empty;
			if (string.IsNullOrWhiteSpace(username) || username.Trim().Length < Constants.Instance.CONST_USERNAME_MIN_SIZE)
			{
				errMsg = Constants.Instance.CONST_ERR_MSG_TOO_SHORT_USERNAME;
				return null;
			}
			var id = !string.IsNullOrWhiteSpace(userId) ? userId : (Settings.SimpleOrGuidPlayerId ? MakeId(username) : MakeId());
			if (Players.ContainsKey(id))
			{
				errMsg = Constants.Instance.CONST_ERR_MSG_SIMILAR_USER_DETECTED;
				return null;
			}
			var p = new Player(username, id);
			Players.Add(p.Id, p);
			return p.Id;
		}

		public Player CheckIfAuthorized(string playerId, out string errMsg)
		{
            var userId = MakeId(playerId);

            errMsg = string.Empty;
			if (string.IsNullOrWhiteSpace(playerId) || !Players.ContainsKey(userId))
			{
				errMsg = Constants.Instance.CONST_ERR_MSG_YOUARE_UNAUTHORIZED;
				return null;
			}
			return Players[userId];
		}

		//MAPS:

		public void RegisterMap(Map map)
		{
			if (string.IsNullOrWhiteSpace(map.Id))
				throw new Exception(Constants.Instance.CONST_ERR_MSG_CANT_CREATE_MAP_WITHOUT_ID);
			if (Maps.ContainsKey(map.Id))
				throw new Exception(Constants.Instance.CONST_ERR_MSG_DUPLICATE_MAP_ID_DETECTED);
			Maps.Add(map.Id, map);
		}

		public Map LookupMap(string id, out string errMsg)
		{
			errMsg = string.Empty;
			var idd = MakeId(id);
			if (!Maps.ContainsKey(idd))
			{
				errMsg = string.Format(Constants.Instance.CONST_ERR_MSG_CANT_FIND_MAP, id);
				return null;
			}
			return Maps[idd];
        }

        public List<string> GetAllMapIds()
        {
            return Maps.Keys.ToList();
        }

        //GAME ROOMS:

        public void CreateGameRoom(Player player, GameRoom gameRoom)
		{
			if (player == null || gameRoom == null)
				return;
			GameRooms.Add(gameRoom.Id, gameRoom);
			player.GameRoomRef = gameRoom;
			gameRoom.ServerRef = this;
		}

		public void RemoveGameRoom(Player leader, GameRoom gameRoom)
		{
			if (leader == null || gameRoom == null)
				return;
			if (gameRoom.Leader.Id == leader.Id)
			{
				gameRoom.ClearPlayers(leader.Id);
				GameRooms.Remove(gameRoom.Id);
				gameRoom.ServerRef = null;
			}
		}

		public string CreateGameRoom(Player player, string name, out string errMsg)
		{
			errMsg = string.Empty;
			if (player.GameRoomRef != null)
			{
				errMsg = Constants.Instance.CONST_ERR_MSG_IS_IN_GAME_ROOM;
				return null;
			}
			var gameRoom = new GameRoom(name, player, this);
			return gameRoom.Id;
		}

		public string CreateGameRoom(string playerId, string name, out string errMsg)
		{
			errMsg = string.Empty;
			var player = LookupPlayer(playerId, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
			{
				return null;
			}
			return CreateGameRoom(player, name, out errMsg);
		}

		public GameRoom[] GetGameRoomList(string playerId, out string errMsg, RoomLookupSettings lookupSettings = null)
		{
			if (lookupSettings == null)
				lookupSettings = new RoomLookupSettings();
			errMsg = string.Empty;

            var player = CheckIfAuthorized(playerId, out errMsg);
            if (player == null)
                return new GameRoom[0];

			if (lookupSettings.CurrentPlayerInside)
			{
				var gr = player.GameRoomRef;
				if (gr != null)
					return new GameRoom[] { gr };
				return new GameRoom[0];
			}
			var query = GameRooms.Values.AsEnumerable();
			if (!string.IsNullOrWhiteSpace(lookupSettings.Id))
				query = query.Where(m => m.Id == lookupSettings.Id);
			return query.ToArray();
		}

		//LOOKUPS:

		public string LookupPlayerId(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
				return null;
			var p = Players.Where(m => m.Value.Username.ToLowerInvariant() == username.Trim().ToLowerInvariant());
			if (p.Count() > 0 && p.First().Value != null)
				return p.First().Key;
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerId"></param>
		/// <param name="itsYou">true if lookup for current user, if trying to find another user - set to false. It depends what message you will get if can't find</param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public Player LookupPlayer(string playerId, out string errMsg, bool itsYou = true)
		{
			errMsg = string.Empty;
            var player = CheckIfAuthorized(playerId, out errMsg);
            if (player == null)
			{
				if (!itsYou)
					errMsg = Constants.Instance.CONST_ERR_MSG_CANT_FIND_SUCH_PLAYER;
				return null;
			}
			return player;
		}

        public GameRoom LookupGameRoom(string playerId, string gameRoomId, out string errMsg)
        {
            errMsg = string.Empty;
            var player = CheckIfAuthorized(playerId, out errMsg);
            if (player == null)
                return null;
            if (!GameRooms.ContainsKey(gameRoomId ?? ""))
            {
                errMsg = string.Format(Constants.Instance.CONST_ERR_MSG_CANT_FIND_GAME_ROOM, gameRoomId);
                return null;
            }
            return GameRooms[gameRoomId];
        }
        
	}
}
