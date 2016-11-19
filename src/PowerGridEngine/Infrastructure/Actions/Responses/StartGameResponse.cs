using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class StartGameResponse: ActionResponse
    {
        public bool IsStarted { get; private set; }

        public StartGameResponse(bool isStarted)
        {
            IsStarted = isStarted;
        }
    }
}
