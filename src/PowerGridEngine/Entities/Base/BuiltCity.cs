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
        public Dictionary<string, BuiltCity> Cities  { get; private set; }

        public User Player { get; private set; }

        public PlayerBuiltCities(User player)
        {
            Player = player;
            Cities = new Dictionary<string, BuiltCity>();
        }

        //todo need to move it outside
        public bool Build(City city)
        {
            var context = GameContext.GetContextByPlayer(Player);
            var builts = context.GameBoard.BuildPlayersCities.Values.SelectMany(m => m.Cities)
                .Where(m => m.Key == city.Id);
            var level = builts.Max(m => m.Value.Level);
            if ((level++) < city.Levels.Max(m => m.Key))
            {
                var cost = city.Levels[level - 1];
                if (GameRule.CanPay(Player, cost))
                {
                    Cities.Add(city.Id, new BuiltCity(city, level));
                    GameRule.PaymentTransaction(Player, cost);
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
