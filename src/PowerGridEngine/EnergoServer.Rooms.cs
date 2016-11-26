using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public partial class EnergoServer
    {
        public static int MaxPlayersInRoom { get; private set; }

        public void CreateGameRoom(User player, GameRoom gameRoom)
        {
            if (player == null || gameRoom == null)
                return;
            GameRooms.Add(gameRoom.Id, gameRoom);
            player.GameRoomRef = gameRoom;
        }

        public GameRoom CreateGameRoom(User player, string name, out string errMsg)
        {
            errMsg = string.Empty;
            if (player.GameRoomRef != null)
            {
                errMsg = Constants.Instance.ErrorMessage.Is_In_Game_Room;
                return null;
            }
            if (!name.CheckIfNameIsOk())
            {
                errMsg = Constants.Instance.ErrorMessage.Wrong_Name;
                return null;
            }
            var gameRoom = new GameRoom(name, player);
            return gameRoom;
        }

        public void RemoveGameRoom(User leader, GameRoom gameRoom)
        {
            if (leader == null || gameRoom == null)
                return;
            if (gameRoom.Leader.Id == leader.Id)
            {
                gameRoom.ClearPlayers(leader.Id);
                GameRooms.Remove(gameRoom.Id);
            }
        }

        public int GetRoomsQty()
        {
            return GameRooms.Count();
        }

        public GameRoom[] GetGameRoomList(User user, RoomLookupSettings lookupSettings = null)
        {
            if (lookupSettings == null)
                lookupSettings = new RoomLookupSettings();
  
            if (user == null)
                return new GameRoom[0];

            if (lookupSettings.CurrentPlayerInside)
            {
                var gr = user.GameRoomRef;
                if (gr != null)
                    return new GameRoom[] { gr };
                return new GameRoom[0];
            }
            var query = GameRooms.Values.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(lookupSettings.Id))
                query = query.Where(m => m.Id == lookupSettings.Id);
            return query.ToArray();
        }

        public GameRoom LookupGameRoom(User user, string gameRoomId, out string errMsg)
        {
            errMsg = string.Empty;
            if (user == null)
                return null;
            if (!GameRooms.ContainsKey(gameRoomId ?? ""))
            {
                errMsg = string.Format(Constants.Instance.ErrorMessage.Cant_Find_Game_Room, gameRoomId);
                return null;
            }
            return GameRooms[gameRoomId];
        }

        public GameRoom TryToLookupRoom(string gameRoomId)
        {
            return GameRooms.ContainsKey(gameRoomId ?? "") ? GameRooms[gameRoomId] : null;
        }
    }
}
