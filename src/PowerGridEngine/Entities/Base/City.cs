using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
	public class City: ParentableBaseEntity<Region>
    {
		public Map MapRef
		{
			get
			{
				return Parent.Parent;
			}
		}
      
        public IDictionary<int, int> Levels
		{
			get
			{
				if (MapRef.Settings.CityLevelRule != null)
				{
					if (MapRef.Settings.OverrideCityLevelsByRule)
						return MapRef.Settings.CityLevelRule.GetLevels();
					else if (TmpLevels == null || TmpLevels.Count() < 1)
						return MapRef.Settings.CityLevelRule.GetLevels();
				}
				if (TmpLevels != null)
					return TmpLevels;
				return new Dictionary<int, int>();
			}
		}

        //what a fuck? why tmp??
		public IDictionary<int, int> TmpLevels { get; private set; }

		/// <summary>
		/// Optional. For make dynamic maps
		/// </summary>
		public Tuple<decimal, decimal> Coords { get; private set; }

		public IDictionary<string, Connector> Connectors
		{
			get
			{
				return MapRef.Connectors
                    .Where(m => m.Key.Contains(string.Format(Constants.Instance.CONST_CONNECTOR_CITY_TEMPLATE, Id)))
			        .ToDictionary(n => n.Key, m => m.Value);
			}
		}

		private void BaseConstructor(Region region = null,
			 ICityLevelRule cityLevelRule = null, Tuple<decimal, decimal> coords = null)
		{
			TmpLevels = new Dictionary<int, int>();
			Coords = coords;
            
            //todo: need to remove it (and parameter too) and move to abstract class
			if (region != null)
				region.AddCity(this);
			if (cityLevelRule != null)
			{
				var lvls = cityLevelRule.GetLevels();
				foreach (var lvl in lvls)
				{
					AddLevel(lvl.Key, lvl.Value);
				}
			}
		}

        public City(string name, ICityLevelRule cityLevelRule = null, Tuple<decimal, decimal> coords = null)
            : base(name, null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Can't create city: unnamed city");
            BaseConstructor(null, cityLevelRule, coords);
        }

        public City(string name, Region region, ICityLevelRule cityLevelRule = null, Tuple<decimal, decimal> coords = null)
            : base(name, region)
        {
            if (string.IsNullOrWhiteSpace(name) || region == null)
                throw new ArgumentException("Can't create city: unnamed city or undentified region");
            BaseConstructor(region, cityLevelRule, coords);
        }

        public City(string name) : base(name, null)
        {
            BaseConstructor();
        }

        public City(string name, Tuple<decimal, decimal> coords) : base(name, null)
        {
            BaseConstructor(coords: coords);
        }

		public City AddLevel(int levelNumber, int cost)
		{
			if (!TmpLevels.ContainsKey(levelNumber))
				TmpLevels.Add(levelNumber, cost);
			return this;
		}

		public City AddLevels(ICityLevelRule cityLevelRule)
		{
			var lvls = cityLevelRule.GetLevels();
			foreach (var lvl in lvls)
				AddLevel(lvl.Key, lvl.Value);
			return this;
		}
	}
}
