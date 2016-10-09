using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class RegionModel : BaseEnergoModel<Region, RegionModelViewOptions>
    {
        public string Id { get { return Entity.Id; } }

        public string Name { get { return Entity.Name; } }

        public bool IsLocked { get { return Entity.IsLocked; } }

        public City[] Cities { get { return Entity.Cities.Values.ToArray(); } }

        public RegionModel(Region entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(RegionModelViewOptions options = null)
        {
            if (options == null)
                options = new RegionModelViewOptions(true);

            var result = new Dictionary<string, object>();
            if (options.Id)
                result.Add("Id", this.Id);
            if (options.Name)
                result.Add("Name", this.Name);
            if (options.IsLocked)
                result.Add("IsLocked", this.IsLocked);
            if (options.Cities)
                result.Add("Cities", this.Cities.Select(m => new CityModel(m).GetInfo(options.CityViewOptions)).ToArray());
            return result;
        }
    }
}
