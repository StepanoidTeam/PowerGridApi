using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class BuildPhase : Phase
    {
        public BuildPhase(StateBatch container) : base(container)
        {

        }

        protected override bool TryToResolve(User user)
        {
            //check if really done (nothing should be DONE before done:) and do some specific phase logic)

            //then run basic done method
            return base.TryToResolve(user);
        }

        public override T RouteAction<T>(UserAction<T> action)
        {
            if (action is BuildCityAction)
                return (T)BuildCity(action as BuildCityAction);

            return (T)new ActionResponse(false, "Unallowable action");
        }

        public ActionResponse BuildCity(BuildCityAction action)
        {
            var board = container.GameContext.GameBoard;
            var wasMoney = container.GameContext.PlayerBoards[action.User.Id].Money;

            var playerCities = new PlayerBuiltCities(action.User);
            if (board.BuildPlayersCities.ContainsKey(action.User.Id))
                playerCities = board.BuildPlayersCities[action.User.Id];
            else
                board.BuildPlayersCities.Add(action.User.Id, playerCities);

            var result = playerCities.Build(action.City);
            if (result)
            {
                var nowMoney = container.GameContext.PlayerBoards[action.User.Id].Money;
                TryToResolve(action.User);
                return new BuildCityResponse(wasMoney - nowMoney);
            }
            return new BuildCityResponse("Can't build here");
        }
    }
}
