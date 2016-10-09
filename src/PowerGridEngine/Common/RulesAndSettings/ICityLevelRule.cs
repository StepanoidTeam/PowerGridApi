using System.Collections.Generic;

namespace PowerGridEngine
{
    public interface ICityLevelRule
    {
        IDictionary<int, int> GetLevels();
    }
}
