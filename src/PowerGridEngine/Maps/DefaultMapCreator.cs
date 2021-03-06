﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public class DefaultMapCreator : IMapCreator
    {
        private static Map map;

        public Map Map
        {
            get
            {
                if (map == null)
                    map = Create();
                
                return map;
            }
        }

        public static void CreateDefaultRegions(Map map)
        {
            var redRegion = new Region("Red", map);
            var blueRegion = new Region("Teal", map);
            var brownRegion = new Region("Brown", map);
            var yelRegion = new Region("Yellow", map);
            var greenRegion = new Region("Green", map);
            var violetRegion = new Region("Violet", map);
        }

        public static void CreateDefaultConnectors(Map map)
        {
            map.AddConnector("omaha", "denver", 7);
            //will not be added because it's duplicate
            map.AddConnector("denver", "omaha", 8);
        }

        private Map Create()
        {
            //City level means how much money will needed to buy city (different cost for each lvl).
            //Levels opened according to STATE(ЭТАП) of game (at begining allowable only first level) 
            var staticMarginCLR = new DefaultStaticMarginCityLevelRule(3, 10, 5);

            var defaultMap = new Map("old_one", new MapSettings()
            {
                CityLevelRule = staticMarginCLR,
                OverrideCityLevelsByRule = true//means that no matter what levels setting will be for 
                                               //regions and cities - anyway it will be overrided by MapSettings.CityLevelRule
                                               //(in case when CityLevelRule is null, this property doesn't works)
            });

            var redRegion = new Region("Red", defaultMap)
                 //if we need to add custom levels for cities:
                 .AddCity(new City("Kansas City")
                      //one variant to add levels:
                      //one by one
                      .AddLevel(1, 10).AddLevel(2, 15).AddLevel(3, 20))
                 //another variant to add levels:
                 //declare cityLevelsRule (see above)
                 //and add levels by this rule:
                 .AddCity(new City("Oklahoma City")
                      .AddLevels(staticMarginCLR))
                 //or even instantly by city constructor
                 .AddCity(new City("Memphis", staticMarginCLR));

            defaultMap.AddCity("Birmincham", "Red")
                .AddCity("New Orleans", "Red")
                .AddCity("Dallas", "Red")
                .AddCity("Houston", "Red");

            var blueRegion = new Region("Blue", defaultMap)
                 .AddCity(new City("Santa Fe", staticMarginCLR))
                 .AddCity(new City("Phoenix"))
                 .AddCity(new City("San Diego"))
                 .AddCity(new City("Los Angeles"))
                 .AddCity(new City("Las Vegas"))
                 .AddCity(new City("Salt Lake City"))
                 .AddCity(new City("San Francisco"));

            var yelRegion = new Region("Yellow", defaultMap);

            yelRegion.AddCity("Fargo", staticMarginCLR)
                 .AddCity("Minneapolis").AddCity("Ouluth")
                 .AddCity("Chicago").AddCity("St. Louis")
                 .AddCity("Cincinnati").AddCity("Knoxville");

            //lightest variant:
            var brownRegion = new Region("Brown", defaultMap)
                .AddCity("Detroit").AddCity("Boston")
                .AddCity("Buffalo").AddCity("New York")
                .AddCity("Pittsburgh").AddCity("Washington").AddCity("Philadelphia");

            var greenRegion = new Region("Green", defaultMap)
              .AddCity("Raleigh").AddCity("Norfolk")
              .AddCity("Savannah").AddCity("Atlanta")
              .AddCity("Tampa").AddCity("Miami").AddCity("Jacksonville");

            var violetRegion = new Region("Violet", defaultMap)
              .AddCity("Omaha").AddCity("Cheyenne")
              .AddCity("Denver").AddCity("Billings")
              .AddCity("Boise").AddCity("Seattle").AddCity("Portland");

            //Another part - connectors between cities
            CreateDefaultConnectors(defaultMap);

            return defaultMap;
        }
    }
}
