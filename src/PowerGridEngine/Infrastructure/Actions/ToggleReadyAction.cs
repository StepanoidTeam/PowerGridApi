using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Contains data required for specific user action in game and actually is a route to specific action
    /// </summary>
    public class ToggleReadyAction : UserAction
    {
        public bool? State { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="state">null - toggle according to current state, otherwise set current state to this state</param>
        public ToggleReadyAction(User user, bool? state = null) : base(user)
        {
            State = state;
        }

        //public override ToggleReadyResponse Run()
        //{
        //    var context = GameContext.GetContextByPlayer(User);
        //    if (User.GameRoomRef.CurrentStage is CreateGameStage)
        //    {
        //        var stage = User.GameRoomRef.CurrentStage as CreateGameStage;
        //        var result = false;
        //        if (State.HasValue)
        //            result = stage.SetReadyMarkTo(User, State.Value);
        //        else
        //            result = stage.ToogleReadyMark(User);
        //    }
        //    return new ToggleReadyResponse("Incorrect game stage");
        //}
    }
}
