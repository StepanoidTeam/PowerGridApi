using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{    
    /// <summary>
    /// Base phase description. Round contains several phases. Active phase is always only one.
    /// </summary>
    public abstract class Phase : State
    {
        private IDictionary<string, bool> _userStates { get; set; }

        public Phase(StateBatch container) : base(container)
        {
        }

        protected override bool TryToResolve(User user)
        {
            var userId = user == null ? "" : user.Id;
            if (_userStates.ContainsKey(userId))
            {
                _userStates[userId] = true;
                _container.GameContext.GameBoard.ChangePlayerTurn();
            }

            if (_userStates.All(m => m.Value))
                return base.TryToResolve(user);             

            return false;
        }

        protected override void Clear()
        {
            _userStates = Players.ToDictionary(k => k.Id, v => false);
            base.Clear();
            //todo need to override it when need to do some specific New Round logic for some phase?
        }
    }
}
