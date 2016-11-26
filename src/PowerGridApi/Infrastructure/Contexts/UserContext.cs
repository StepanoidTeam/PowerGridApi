using PowerGridEngine;
using System;

namespace PowerGridApi
{
    public class UserContext
    {
        public User User { get; private set; }

        public UserContext(User user)
        {
            User = user;
        }
    }
}
