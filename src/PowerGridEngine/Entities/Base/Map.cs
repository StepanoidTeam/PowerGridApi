using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{	
	public class Map: ChildableBaseEntity<Dictionary<string, Region>>
    {
        public IDictionary<string, Region> Regions { get { return Childs; } }

        public IDictionary<string, City> Cities
        {
            get
            {
                return Regions.SelectMany(m => m.Value.Cities).ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public MapSettings Settings { get; private set; }

        public IDictionary<string, Connector> Connectors { get; private set; }

		public Map(string name, MapSettings settings = null): base(name)
		{
			Settings = settings;
			if (Settings == null)
				Settings = new MapSettings();
			//Regions = new Dictionary<string, Region>();
			Connectors = new Dictionary<string, Connector>();
			EnergoServer.Current.RegisterMap(this);
		}

		public Map AddRegion(Region region)
		{
			if (!Regions.ContainsKey(region.Id))
			{
				foreach (var city in region.Cities)
					if (Cities.ContainsKey(city.Key))
						throw new Exception("Found city duplicates on the map");
				//region.MapRef = this;
				Regions.Add(region.Id, region);
			}
			return this;
		}

		public Map AddConnector(Connector connector)
		{
			if (!Connectors.ContainsKey(connector.Id))
			{
				connector.MapRef = this;
				Connectors.Add(connector.Id, connector);
			}
			return this;
		}

		public Map AddConnector(string city1Key, string city2Key, int cost)
		{
			var city1 = LookupCity(city1Key);
			var city2 = LookupCity(city2Key);
			if (city1 == null || city2 == null)
				throw new Exception(string.Format("Can't find one of cities: '{0}', '{1}'", city1Key, city2Key));
			var connector = new Connector(cost, city1, city2, this);
			return this;
		}

		public Map AddRegion(string name)
		{
			var region = new Region(name, this);
			return this;
		}

		public Map AddRegion(string name, out Region region)
		{
			region = new Region(name, this);
			return this;
		}

		public Region LookupRegion(string name)
		{
			var n = name.NormalizeId();
			if (Regions.ContainsKey(n))
				return Regions[n];
			return null;
		}

		public Region LookupOrCreateRegion(string name)
		{
			var region = LookupRegion(name);
			if (region == null)
				AddRegion(name, out region);
			return region;
		}

		public City LookupCity(string name)
		{
			var n = name.NormalizeId();
			if (Cities.ContainsKey(n))
				return Cities[n];
			return null;
		}

		public Map AddCity(string name, Region region, ICityLevelRule rule = null, Tuple<decimal, decimal> coords = null)
		{
			if (string.IsNullOrWhiteSpace(name) || region == null)
				throw new ArgumentException("Can't add undefined city or udentified region");
			var city = new City(name, region, rule, coords);
			return this;
		}

		public Map AddCity(string name, string regionName, ICityLevelRule rule = null, Tuple<decimal, decimal> coords = null)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(regionName))
				throw new ArgumentException("Can't add undefined city or udentified region");
			var region = LookupOrCreateRegion(regionName);
			AddCity(name, region, rule, coords);
			return this;
		}

		public Map AddCity(City city, Region region = null)
		{
			if (city == null)
				throw new ArgumentException("Can't add undefined city");
			if (region == null)
			{
				if (city.Parent == null)
					throw new ArgumentException("Can't add city to udentified region");
				AddRegion(city.Parent);
			}
			else
			{
				if (city.Parent != null)
					throw new ArgumentException("This city already inside some region");
				AddRegion(region);
				region.AddCity(city);
			}
			return this;
		}

		/// <summary>
		/// Check map for unique names (regions and cities)
		/// </summary>
		/// <returns></returns>
		public bool Check()
		{
			var flag = true;
			var lst = new Dictionary<string, int>();
			foreach (var region in Regions)
			{
				foreach (var city in region.Value.Cities)
					if (!lst.ContainsKey(city.Key))
						lst.Add(city.Key, 1);
					else
					{
						lst[city.Key] += 1;
						flag = false;
					}
			}
			return flag;
		}
    }
}
