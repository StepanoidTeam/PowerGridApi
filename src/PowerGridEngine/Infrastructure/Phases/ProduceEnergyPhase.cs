using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class ProduceEnergyPhase: Phase
    { 
        public ProduceEnergyPhase(StateBatch container): base(container)
        {
        }

        protected override bool TryToResolve(User user)
        {
            //check if really done (nothing should be DONE before done:) and do some specific phase logic)

            //then run basic done method
            return base.TryToResolve(user);
        }

        public override T RouteAction<T>(UserAction<T> action)
        {
            return (T)new ActionResponse(false, "Unallowable action");
        }
    }
}
