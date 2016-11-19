using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{    
    /// <summary>
    /// First stage of game
    /// </summary>
    public class FirstStage: Stage
    {
        private Round Round { get; set; }

        /// <summary>
        /// Stage will finished when there will be <userCount> users and every one will check their ready mark
        /// </summary>
        /// <param name="gameContext"></param>
        /// <param name="userCount"></param>
        public FirstStage(GameStages container) : base(container)
        {
            Round = new Round(container.GameContext)
                .Add<BuildPhase>()
                .Add<ProduceEnergyPhase>()
                .Start();
        }

        //protected override bool TryToResolve()
        //{
        //    if (readyMarks.Count() == Players.Count && readyMarks.Values.All(m => true))
        //    {
        //        return base.TryToResolve();
        //    }
        //    return false;
        //}

        public override T RouteAction<T>(UserAction<T> action)
        {
            return Round.CurrentPhase.RouteAction(action);
        }

        //public ActionResponse StartGame(StartGameAction action)
        //{
        //    var result = TryToResolve();

        //    if (result)
        //    {
        //        //todo move this out
        //        foreach (var p in container.GameContext.PlayerBoards)
        //            GameRule.PaymentTransaction(p.Value.PlayerRef, 50);
        //        GameRule.ChangeTurnOrder(container.GameContext);
        //    }

        //    return new StartGameResponse(result);
        //}

    }
}
