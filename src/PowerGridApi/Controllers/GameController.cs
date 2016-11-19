using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using Swashbuckle.SwaggerGen.Annotations;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    /// <summary>
    /// Game (started and currently in process) related actions
    /// </summary>
    [Route("api/[controller]")]
    public class GameController : BaseController
    {
        /// <summary>
        /// Get all allowable actions for current user according to current game state
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotYourTurn")]
        [HttpGet("AllowedActions")]
        [RestrictByState(UserState.InGame)]
        public async Task<IActionResult> GetAllowedActions([FromHeader]string authToken)
        {
            var errMsg = string.Empty;
            //todo it's shit to get allowed actions from GAME BOARD
            var lst = GameContext.GetContextByPlayer(UserContext.User).GameBoard.GetAllowedActions(UserContext.User.AuthToken, out errMsg);
            return await GenericResponse(errMsg, lst, ResponseType.NotYourTurn);
        }

        /// <summary>
        /// Do specific game action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost("DoAction")]
        public async Task<IActionResult> DoAction([FromHeader]string authToken, DoActionModel action)
        {
            //todo Refactor in completely
            //var errMsg = string.Empty;
            //var gbRef = UserContext.User.GameRoomRef.GameBoardRef;
            //switch (action.Action)
            //{
            //    case GameActionEnum.AuctionPass:
            //        if (!gbRef.AuctionPass(UserContext.User.AuthToken, out errMsg))
            //            return await GenericResponse(errMsg);
            //        return await GenericResponse(null, true);
            //}
            return await GenericResponse(ResponseType.UnexpectedError, "Incorrect action");
        }

        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotInGame")]
        [RestrictByState(UserState.InGame)]
        [HttpPost("BuildCity")]
        public async Task<IActionResult> BuildCity([FromHeader]string authToken, [FromBody]BuildCityModel buildCityModel)
        {
            var user = UserContext.User;

            var context = GameContext.GetContextByPlayer(user);
            var citites = context.GameBoard.MapRef.Cities;
            if (!citites.ContainsKey(buildCityModel.CityId))
                return await ErrorResponse("No such city", ResponseType.InvalidModel);

            var city = context.GameBoard.MapRef.Cities[buildCityModel.CityId];
            var response = EnergoServer.Current.RouteAction(new BuildCityAction(user, city));

            if (!response.IsSuccess)
                return await ErrorResponse(response.ErrorMsg, ResponseType.InvalidModel);
            return await SuccessResponse(response);
        }

    }
}
