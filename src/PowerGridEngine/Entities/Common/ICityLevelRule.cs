using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public interface ICityLevelRule
    {
        IDictionary<int, int> GetLevels();
    }
}
