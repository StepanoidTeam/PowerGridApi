using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class StartGameResponse: ActionResponse
    {
        public StartGameResponse()
        {
        }

        public StartGameResponse(string errMsg): base(false, errMsg)
        {
        }
    }
}
