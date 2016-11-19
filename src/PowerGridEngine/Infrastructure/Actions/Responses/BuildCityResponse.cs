using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class BuildCityResponse : ActionResponse
    {
        public int SpentMoney { get; private set; }

        public BuildCityResponse(int spentMoney)
        {
            SpentMoney = spentMoney;
        }

        public BuildCityResponse(string errMsg): base(false, errMsg)
        {
        }
    }
}
