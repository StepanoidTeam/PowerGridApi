using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class LoadMapCity
    {
        public string Name { get; set; }
        public string RegionKey { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
    }

    public class LoadMapModel
    {
        public LoadMapCity[] Cities { get; set; }
    }
}
