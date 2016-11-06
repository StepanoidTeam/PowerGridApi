using System;

namespace PowerGridApi
{
    public enum UserState
    {
        Free, //Not in room, not in game
        InRoom,
        InGame
    }

    public class RestrictByStateAttribute : Attribute
    {
        public UserState UserState { get; set; }

        public RestrictByStateAttribute(UserState userState)
        {
            UserState = userState;
        }
    }
}
