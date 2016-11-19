using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class StartGameAction : UserAction<StartGameResponse>
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public StartGameAction(User user) : base(user)
        {
        }
    }
}
