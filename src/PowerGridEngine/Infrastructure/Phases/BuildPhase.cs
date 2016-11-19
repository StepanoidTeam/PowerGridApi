using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class BuildPhase : Phase
    {
        public BuildPhase(Round container) : base(container)
        {

        }

        public override void Done(User user)
        {
            //check if really done (nothing should be DONE before done:) and do some specific phase logic)

            //then run basic done method
            base.Done(user);
        }

        public override T RouteAction<T>(UserAction<T> action)
        {
            if (action is BuildCityAction)
                return (T)BuildCity(action as BuildCityAction);

            return (T)new ActionResponse(false, "Unallowable action");
        }

        public ActionResponse BuildCity(BuildCityAction action)
        {
            var board = Container.GameContext.GameBoard;
            var wasMoney = Container.GameContext.PlayerBoards[action.User.Id].Money;
            var result = board.BuildPlayersCities[action.User.Id].Build(action.City);
            if (result)
            {
                var nowMoney = Container.GameContext.PlayerBoards[action.User.Id].Money;
                Done(action.User);
                return new BuildCityResponse(nowMoney - wasMoney);
            }
            return new BuildCityResponse("Can't build here");
        }
    }
}
