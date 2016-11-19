using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Base stage description. Stage is specific game state, while it active only some specific for stage actions are allowable or
    /// if it's some game stage (not generic) - it will change some game rules.
    /// </summary>
    public abstract class Stage : IUserActionHandler
    {
        protected GameStages container;

        protected List<User> Players
        {
            get
            {
                return container.GameContext.Players;
            }
        }

        protected bool ReturnError(string err, out string errMsg)
        {
            errMsg = err;
            return false;
        }

        public bool IsFinished { get; private set; }

        public Stage(GameStages container)
        {
            this.container = container;
        }

        /// <summary>
        /// According to Stage logic this method should check if Stage is done 
        /// </summary>
        protected virtual bool TryToResolve()
        {
            IsFinished = true;
            container.Next();
            return IsFinished;
        }

        public abstract T RouteAction<T>(UserAction<T> action) where T : ActionResponse;
    }
}
