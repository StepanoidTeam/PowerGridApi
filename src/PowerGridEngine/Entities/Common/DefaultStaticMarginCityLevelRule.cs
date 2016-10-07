using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public class DefaultStaticMarginCityLevelRule : ICityLevelRule
    {
        public IDictionary<int, int> Levels { get; private set; }

        public DefaultStaticMarginCityLevelRule(int levelsCount, int firstCost, int costMargin)
        {
            Levels = new Dictionary<int, int>();
            var cost = firstCost;
            for (int i = 0; i < levelsCount; i++, cost += costMargin)
                Levels.Add(i, cost);
        }

        public IDictionary<int, int> GetLevels()
        {
            return Levels;
        }
    }
}
