using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class MapModel : BaseEnergoModel<Map, MapModelViewOptions>
    {
        public City[] Cities { get { return Entity.Cities.Values.ToArray(); } }

        public Region[] Regions { get { return Entity.Regions.Values.ToArray(); } }

        public Connector[] Connectors { get { return Entity.Connectors.Values.ToArray(); } }

        public MapModel(Map entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(MapModelViewOptions options = null)
        {
            if (options == null)
                options = new MapModelViewOptions(true);

            var result = new Dictionary<string, object>();
            if (options.Cities)
                result.Add("Cities", this.Cities.Select(m => new CityModel(m).GetInfo(options.CityViewOptions)).ToArray());
            if (options.Regions)
                result.Add("Regions", this.Regions.Select(m => new RegionModel(m).GetInfo(options.RegionViewOptions)).ToArray());
            if (options.Connectors)
                result.Add("Connectors", this.Connectors.Select(m => new ConnectorModel(m).GetInfo(options.ConnectorViewOptions)).ToArray());
            return result;
        }
    }
}
