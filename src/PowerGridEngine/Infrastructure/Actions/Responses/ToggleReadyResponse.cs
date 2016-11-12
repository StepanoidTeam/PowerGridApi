using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class ToggleReadyResponse: ActionResponse
    {
        public bool CurrentState { get; private set; }

        public ToggleReadyResponse(bool newState)
        {
            CurrentState = newState;
        }

        public ToggleReadyResponse(string errMsg): base(false, errMsg)
        {
        }
    }
}
