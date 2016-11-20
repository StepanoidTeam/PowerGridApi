using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class PlayerBuiltCities
    {
        /// <summary>
        /// key is city Id
        /// </summary>
        public List<BuiltCity> Cities  { get; private set; }

        public User Player { get; private set; }

        public PlayerBuiltCities(User player)
        {
            Player = player;
            Cities = new List<BuiltCity>();
        }

        //todo need to move it outside
        public bool Build(City city)
        {
            var context = GameContext.GetContextByPlayer(Player);
            var builts = context.GameBoard.BuildPlayersCities.Values.SelectMany(m => m.Cities)
                .Where(m => m.City.Id == city.Id);
            var level = builts.Any() ? builts.Max(m => m.Level) : -1;
            level++;
            if (level <= city.Levels.Max(m => m.Key))
            {
                var cost = city.Levels[level];
                if (GameRule.CanPay(Player, cost))
                {
                    Cities.Add(new BuiltCity(city, level));
                    GameRule.PaymentTransaction(Player, -cost);
                    return true;
                }
            }
            return false;
        }
    }

    public class BuiltCity
    {
        public City City { get; private set; }

        public int Level { get; private set; }

        public BuiltCity( City city, int level)
        {
            City = city;
            Level = level;
        }
	}
}
