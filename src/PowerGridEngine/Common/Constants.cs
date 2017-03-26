
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
                {
                    _instance = new Constants();
                    _instance.ErrorMessage = new ErrorMessages();
                }
                return _instance;
            }
        }

        public const string CONST_DEFAULT_MAP_ID = "default";

        public readonly string CONST_CONNECTOR_CITY_TEMPLATE = "[{0}]";
        public readonly string CONST_CONNECTOR_TEMPLATE = "[{0}]_TO_[{1}]";

        public readonly int CONST_USERNAME_MIN_SIZE = 3;

        public ErrorMessages ErrorMessage;

        public class ErrorMessages
        {
            public readonly string Not_Everybody_Checked_ReadyMark = "Not everybody checked their ready marks.";
            public readonly string Wrong_Name = "Wrong name.";
            public readonly string YouAre_Unauthorized = "You are unauthorized.";
            public readonly string Similar_User_Detected = "Similar user in the system, please change your name.";
            public readonly string Too_Short_Username = "Empty or too short username (should be at least 3 symbols).";
            public readonly string Cant_Create_Map_Without_Id = "Can't create map with udentified Id.";
            public readonly string Duplicate_Map_Id_Detected = "Duplicated map Id.";
            public readonly string Cant_Find_Map = "Can't find map with this Id: {0}";
            public readonly string Is_In_Game_Room = "Is in Game Room already.";
            public readonly string Not_In_Room = "Not in room.";
            public readonly string Cant_Find_Such_User = "Can't find such player.";
            public readonly string Cant_Find_Game_Room = "Can't find game room by this Id: {0}";
            public readonly string Cant_Join_To_Game_In_Proc = "You can't join to the game in process.";
            public readonly string Already_In_This_Game = "You already in this game.";
            public readonly string Already_In_The_Game = "You already in the game.";
            public readonly string YouAre_Outside_Of_Game_Rooms = "You are outside of game rooms.";
            public readonly string YouAre_Not_In_Game = "You are not in game.";
            public readonly string You_Cant_Leave_This_Game_Room = "You are not in room.";
            public readonly string There_No_Such_User = "There no such user.";
            public readonly string You_Cant_Kick_This_User = "You can't kick this user.";
            public readonly string User_Cant_Be_Null = "Player can't be null.";
            public readonly string YouAre_Not_In_This_Game = "You are not inside this game.";
            public readonly string Only_Leader_Can_Kick = "Only leader can kick user.";
            public readonly string Game_Not_In_Status = "Game not in {0} status.";
            public readonly string Is_Not_Yout_Turn = "It's not your turn now. Player {0} turn.";
        }
    }
}
