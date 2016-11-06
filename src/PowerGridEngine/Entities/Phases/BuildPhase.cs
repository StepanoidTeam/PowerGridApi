using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class BuildPhase: Phase
    {
        public BuildPhase()
        {

        }

        public BuildPhase(IEnumerable<User> users) : base(users)
        {
        }

        public override void Done(User user)
        {
            //check if really done (nothing should be DONE before done:) and do some specific phase logic)

            //then run basic done method
            base.Done(user);
        }

    }
}
