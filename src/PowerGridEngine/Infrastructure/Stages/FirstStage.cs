using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{    
    /// <summary>
    /// First stage of game
    /// </summary>
    public class FirstStage: State
    {
        private StateBatch Round { get; set; }

        /// <summary>
        /// Stage will finished when there will be <userCount> users and every one will check their ready mark
        /// </summary>
        /// <param name="gameContext"></param>
        /// <param name="userCount"></param>
        public FirstStage(StateBatch container) : base(container)
        {
            Round = new StateBatch(container.GameContext)
                .Add<BuildPhase>()
                .Add<ProduceEnergyPhase>();
        }

        public override void Begin()
        {
            base.Begin();
            Round.StartRound();
        }

        public override T RouteAction<T>(UserAction<T> action)
        {
            return Round.Current.RouteAction(action);
        }

    }
}
