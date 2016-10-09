using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class CityModel : BaseEnergoModel<City, CityModelViewOptions>
    {
        public string Id { get { return Entity.Id; } }

        public string Name { get { return Entity.Name; } }

        public string RegionKey { get { return Entity.Parent.Id; } }

        public string RegionName { get { return Entity.Parent.Name; } }

        public decimal CoordX { get { return Entity.Coords == null ? 0m : Entity.Coords.Item1; } }

        public decimal CoordY { get { return Entity.Coords == null ? 0m : Entity.Coords.Item2; } }

        public int[] Levels { get { return Entity.Levels.OrderBy(m => m.Key).Select(m => m.Value).ToArray(); } }

        public object[] GetConntectors(ConnectorModelViewOptions options)
        {
            return Entity.Conntectors.Select(m => new ConnectorModel(m.Value).GetInfo(options)).ToArray();
        }

        public CityModel(City entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(CityModelViewOptions options = null)
        {
            if (options == null)
                options = new CityModelViewOptions(true);

            var result = new Dictionary<string, object>();
            if (options.Id)
                result.Add("Id", this.Id);
            if (options.Name)
                result.Add("Name", this.Name);
            if (options.RegionKey)
                result.Add("RegionKey", this.RegionKey);
            if (options.RegionName)
                result.Add("RegionName", this.RegionName);
            if (options.Coords)
            {
                result.Add("CoordX", this.CoordX);
                result.Add("CoordY", this.CoordY);
            }
            if (options.Levels)
                result.Add("Levels", this.Levels);
            if (options.Conntectors)
                result.Add("Conntectors", this.GetConntectors(options.ConnectorViewOptions));
            return result;
        }
    }
}
