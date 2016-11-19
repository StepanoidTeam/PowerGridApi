using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{    
    /// <summary>
    /// Base phase description. Round contais several phases. Active phase is always only one.
    /// </summary>
    public abstract class Phase : IUserActionHandler
    {
        protected Round Container { get; private set; }

        private IDictionary<string, bool> _userStates { get; set; }

        public bool IsFinished { get; private set; }

        public Phase(Round container)
        {
            Container = container;
            Init();
        }

        public void Init()
        {
            _userStates = Container.GameContext.Players.ToDictionary(k => k.Id, v => false);
        }

        public virtual void Done(User user)
        {
            var userId = user == null ? "" : user.Id;
            if (_userStates.ContainsKey(userId))
                _userStates[userId] = true;
            IsFinished = _userStates.All(m => m.Value);
        }

        public void StartNewRound()
        {
            foreach (var key in _userStates.Keys)
                _userStates[key] = false;
            //todo need to override it when need to do some specific New Round logic for some phase?
        }

        public abstract T RouteAction<T>(UserAction<T> action) where T : ActionResponse;
    }
}
