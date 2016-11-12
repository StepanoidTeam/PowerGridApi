using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Base stage description. Stage is specific game state, while it active only some specific for stage actions are allowable or
    /// if it's some game stage (not generic) - it will change some game rules.
    /// </summary>
    public abstract class Stage: IUserActionHandler
    {
        protected GameContext gameContext;

        protected List<User> Players
        {
            get
            {
                return gameContext.Players;
            }
        }

        protected bool ReturnError(string err, out string errMsg)
        {
            errMsg = err;
            return false;
        }

        public bool IsFinished { get; private set; }

        public Stage(GameContext _gameContext)
        {
            gameContext = _gameContext;
        }

        protected bool Done()
        {
            IsFinished = true;
            return IsFinished;
        }

        /// <summary>
        /// According to Stage logic this method should check if Stage is done and if yes - change IsFinished to true
        /// </summary>
        protected abstract bool CheckIfDone();

        public abstract T RouteAction<T>(UserAction action) where T : ActionResponse;
    }
}
