using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Contains data required for specific user action in game and actually is a route to specific action
    /// </summary>
    public abstract class UserAction
    {
        public User User { get; private set; }

        protected UserAction(User user)
        {
            User = user;
        }
        //public abstract T Run();
    }
}
