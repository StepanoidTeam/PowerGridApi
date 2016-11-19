using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{    
    /// <summary>
    /// Stage for preparing user for game. Room should be filled by needed user qty and all users should check Ready mark
    /// </summary>
    public class CreateGameStage: Stage
    {
        private IDictionary<string, bool> readyMarks { get; set; }

        /// <summary>
        /// Stage will finished when there will be <userCount> users and every one will check their ready mark
        /// </summary>
        /// <param name="gameContext"></param>
        /// <param name="userCount"></param>
        public CreateGameStage(GameStages container) : base(container)
        {
            readyMarks = new Dictionary<string, bool>();
        }

        protected override bool TryToResolve()
        {
            if (readyMarks.Count() == Players.Count && readyMarks.Values.All(m => true))
            {
                return base.TryToResolve();
            }
            return false;
        }

        private bool setReadyMark(string userId, bool state)
        {
            if (readyMarks.ContainsKey(userId))
                readyMarks[userId] = state;
            else
                readyMarks.Add(userId, state);
            return state;
        }

        public bool GetReadyMark(string userId)
        {
            if (readyMarks.ContainsKey(userId))
                return readyMarks[userId];
            return false;
        }

        public override T RouteAction<T>(UserAction<T> action)
        {
            if (action is ToggleReadyAction)
                return (T)ToogleReadyMark(action as ToggleReadyAction);
            if (action is StartGameAction)
                return (T)StartGame(action as StartGameAction);

            return (T)new ActionResponse(false, "Unallowable action");
        }

        public ActionResponse ToogleReadyMark(ToggleReadyAction action)
        {
            var userId = action.User.Id;
            //if (player == null)
            //    return ReturnError(Constants.Instance.ErrorMessage.User_Cant_Be_Null, out errMsg);
            //if (player.GameRoomRef == null)
            //    return ReturnError(Constants.Instance.ErrorMessage.YouAre_Outside_Of_Game_Rooms, out errMsg);
            var players = action.User.GameRoomRef.Players;
            if (!players.ContainsKey(userId))
                return new ActionResponse(false, "Unexpected error");
            //if (!players.ContainsKey(player.Id))
            //    return ReturnError(Constants.Instance.ErrorMessage.YouAre_Not_In_This_Game, out errMsg);

            var result = false;
            if (action.State.HasValue)
                result = setReadyMark(userId, action.State.Value);
            else
                result = setReadyMark(userId, !GetReadyMark(userId));
            return new ToggleReadyResponse(result);
        }

        public ActionResponse StartGame(StartGameAction action)
        {
            var result = TryToResolve();

            if (result)
            {
                //todo move this out
                foreach (var p in container.GameContext.PlayerBoards)
                    GameRule.PaymentTransaction(p.Value.PlayerRef, 50);
                GameRule.ChangeTurnOrder(container.GameContext);
            }

            return new StartGameResponse(result);
        }

    }
}
