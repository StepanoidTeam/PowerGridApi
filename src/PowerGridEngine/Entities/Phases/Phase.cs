using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{    
    /// <summary>
    /// Base phase description. Round contais several phases. Active phase is always only one.
    /// </summary>
    public abstract class Phase
    {
        private IDictionary<string, bool> _userStates { get; set; }

        public bool IsFinished { get; private set; }

        public Phase()
        {

        }

        public Phase(IEnumerable<User> users)
        {
            _userStates = users.ToDictionary(k => k.Id, v => false);
        }

        public void Init(IEnumerable<User> users)
        {
            _userStates = users.ToDictionary(k => k.Id, v => false);
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

    }
}
