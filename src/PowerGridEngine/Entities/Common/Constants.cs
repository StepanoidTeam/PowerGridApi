using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public class Constants
    {
        private static Constants _instance;

        public static Constants Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Constants();
                return _instance;
            }
        }

        public const string CONST_DEFAULT_MAP_ID = "default";

        public readonly string CONST_CONNECTOR_CITY_TEMPLATE = "[{0}]";
        public readonly string CONST_CONNECTOR_TEMPLATE = "[{0}]_TO_[{1}]";

        public readonly int CONST_USERNAME_MIN_SIZE = 3;
       
        public readonly string CONST_ERR_MSG_YOUARE_UNAUTHORIZED = "You are unauthorized.";
        public readonly string CONST_ERR_MSG_SIMILAR_USER_DETECTED = "Similar user in the system, please change your name.";
        public readonly string CONST_ERR_MSG_TOO_SHORT_USERNAME = "Empty or too short username (should be at least 3 symbols).";
        public readonly string CONST_ERR_MSG_CANT_CREATE_MAP_WITHOUT_ID = "Can't create map with udentified Id.";
        public readonly string CONST_ERR_MSG_DUPLICATE_MAP_ID_DETECTED = "Duplicated map Id.";
        public readonly string CONST_ERR_MSG_CANT_FIND_MAP = "Can't find map with this Id: {0}";
        public readonly string CONST_ERR_MSG_IS_IN_GAME_ROOM = "Is in Game Room already.";
        public readonly string CONST_ERR_MSG_CANT_FIND_SUCH_PLAYER = "Can't find such player.";
        public readonly string CONST_ERR_MSG_CANT_FIND_GAME_ROOM = "Can't find game room by this Id: {0}";
        public readonly string CONST_ERR_MSG_CANT_JOIN_TO_GAME_IN_PROC = "You can't join to the game in process.";
        public readonly string CONST_ERR_MSG_ALREADY_IN_THIS_GAME = "You already in this game.";
        public readonly string CONST_ERR_MSG_ALREADY_IN_THE_GAME = "You already in the game.";
        public readonly string CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS = "You are outside of game rooms.";
        public readonly string CONST_ERR_MSG_YOU_CANT_LEAVE_THIS_GAME_ROOM = "You can't leave game room.";
        public readonly string CONST_ERR_MSG_THERE_NO_SUCH_USER = "There no such user.";
        public readonly string CONST_ERR_MSG_YOU_CANT_KICK_THIS_USER = "You can't kick this user.";
        public readonly string CONST_ERR_MSG_PLAYER_CANT_BE_NULL = "Player can't be null.";
        public readonly string CONST_ERR_MSG_YOU_ARE_NOT_IN_THIS_GAME = "You are not inside this game.";
        public readonly string CONST_ERR_MSG_ONLY_LEADER_CAN_KICK = "Only leader can kick user.";
		public readonly string CONST_ERR_MSG_GAME_NOT_IN_STATUS = "Game not in {0} status.";
		public readonly string CONST_ERR_MSG_IS_NOT_YOUR_TURN = "It's not your turn now. Player {0} turn.";
    }
}
