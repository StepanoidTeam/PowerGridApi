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

        public void CreateGameRoom(User player, GameRoom gameRoom)
        {
            if (player == null || gameRoom == null)
                return;
            GameRooms.Add(gameRoom.Id, gameRoom);
            player.GameRoomRef = gameRoom;
            gameRoom.ServerRef = this;
        }

        public void RemoveGameRoom(User leader, GameRoom gameRoom)
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

        public string CreateGameRoom(User player, string name, out string errMsg)
        {
            errMsg = string.Empty;
            if (player.GameRoomRef != null)
            {
                errMsg = Constants.Instance.ErrorMessage.Is_In_Game_Room;
                return null;
            }
            var gameRoom = new GameRoom(name, player, this);
            return gameRoom.Id;
        }

        public string CreateGameRoom(string authToken, string name, out string errMsg)
        {
            errMsg = string.Empty;
            var user = LookupUserByAuthToken(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                return null;
            }
            return CreateGameRoom(user, name, out errMsg);
        }

        public GameRoom[] GetGameRoomList(string authToken, out string errMsg, RoomLookupSettings lookupSettings = null)
        {
            if (lookupSettings == null)
                lookupSettings = new RoomLookupSettings();
            errMsg = string.Empty;

            var player = FindUserByAuthToken(authToken, out errMsg);
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

        public GameRoom LookupGameRoom(string authToken, string gameRoomId, out string errMsg)
        {
            errMsg = string.Empty;
            var player = FindUserByAuthToken(authToken, out errMsg);
            if (player == null)
                return null;
            if (!GameRooms.ContainsKey(gameRoomId ?? ""))
            {
                errMsg = string.Format(Constants.Instance.ErrorMessage.Cant_Find_Game_Room, gameRoomId);
                return null;
            }
            return GameRooms[gameRoomId];
        }
    }
}
