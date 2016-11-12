using System;
using System.Collections.Generic;

namespace PowerGridEngine
{
    
    public class Region: FullBaseEntity<Map, Dictionary<string,City>>
    {
        public IDictionary<string, City> Cities
        {
            get
            {
                return this.Childs;
            }
        }

        //It depends of qty of users. In case there are low users - not all regions will be unlocked.
        //This will related to how much resource will be delivered to the market from begining (and maybe later) 
        
        public bool IsLocked { get; set; }

        public Region(string name, Map map): base(name, map)
        {
            //need to move it into Base
            map.AddRegion(this);
        }

        public Region AddCity(string name, ICityLevelRule cityLvlRule = null, Tuple<decimal, decimal> coords = null)
        {
            var city = new City(name, this, cityLvlRule, coords);
            return this;
        }

        public Region AddCity(City city)
        {
            if (city == null)
                return this;

            if (string.IsNullOrWhiteSpace(city.Id))
                ;// ServerContext.Current.Logger.Log("Region->AddCity->Trying to add city without name");
            else if (Parent.Cities.ContainsKey(city.Id))
                ;// ServerContext.Current.Logger.Log("Region->AddCity->Trying to add duplicate city (in Map context): {0}", city.Name);
            else
            {
                Cities.Add(city.Id, city);
                //move this shit into Base
                if (city.Parent == null)
                    city.Parent = this;
            }

            return this;
        }
    }
}
