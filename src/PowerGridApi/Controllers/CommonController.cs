using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.SwaggerGen.Annotations;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
	/// <summary>
	/// Various additional methods
	/// </summary>
	[Route("api/")]
	public class CommonController : BaseController
    {
        /// <summary>
        /// Get API version
        /// </summary>
        [AllowAnonymous]
        [HttpGet("Version")]
		public async Task<IActionResult> GetVersion()
		{
            return await SuccessResponse(Version);
		}

        /// <summary>
        /// Get status of active for current user game
        /// </summary>
        /// <returns></returns>
        [RestrictByState(UserState.InGame)]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotInGame")]
        [HttpGet("Game/Status")]
		public async Task<IActionResult> GetGameStatus([FromHeader]string authToken)
		{
            var user = UserContext.User;

            var gameBoard = GameContext.GetContextByPlayer(user).GameBoard;
            //to do view model
            var gameBoardModel = new GameBoardModel(gameBoard);
            var result = await Task.Run(() => { return gameBoardModel.GetInfo(); });

            return await SuccessResponse(result);
		}

        /// <summary>
        /// Player info
        /// </summary>
        /// <returns></returns>        
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [HttpPost("User/Status")]
        public async Task<IActionResult> GetUserInfo([FromHeader]string authToken, [FromBody]UserModelViewOptions viewOptions)
        {
            var userModel = new UserModel(UserContext.User);

            var responseGetter = SuccessResponse(() =>
            {
                return userModel.GetInfo(viewOptions);
            });

            return await responseGetter();
        }

	}
}
