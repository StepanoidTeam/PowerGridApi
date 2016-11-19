using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Contains data required for specific user action in game and actually is a route to specific action
    /// </summary>
    public abstract class UserAction<T> where T : ActionResponse
    {
        public User User { get; private set; }

        protected UserAction(User user)
        {
            User = user;
        }

        //public abstract T Run();

        public T CreateErrorResponse(string errMessage)
        {
            return (T)Activator.CreateInstance(typeof(T), errMessage);
        }
    }
}
