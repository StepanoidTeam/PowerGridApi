using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public class MapSettings
    {
        public bool OverrideCityLevelsByRule { get; set; }
        public ICityLevelRule CityLevelRule { get; set; }
    }
}
