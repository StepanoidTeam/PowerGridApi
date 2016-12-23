using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Identify specific game state. Game could contains several layers of states, because each state of zero layer could
    /// contains one or more inner states. Also states could be combined into specific batch state for case after Last state 
    /// in some conditions it should again start First state. At same time could be active only one state on same layer.
    /// While it's active only some specific actions are allowable and active some specific game rules.
    /// </summary>
    public abstract class State : IUserActionHandler
    {
        protected StateBatch container;

        protected IEnumerable<User> Players
        {
            get
            {
                return container.GameContext.Players;
            }
        }

        public bool IsFinished { get; protected set; }

        public State(StateBatch container)
        {
            this.container = container;
            Clear();
        }

        protected virtual void Clear()
        {
            IsFinished = false;
        }

        /// <summary>
        /// According to State logic this method should check if Stage is done 
        /// </summary>
        protected virtual bool TryToResolve(User user)
        {
            IsFinished = true;
            container.Next();
            return IsFinished;
        }

        public virtual void Begin()
        {
            Clear();
        }

        public abstract T RouteAction<T>(UserAction<T> action) where T : ActionResponse;
    }
}
