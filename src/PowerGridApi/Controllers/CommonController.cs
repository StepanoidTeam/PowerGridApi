﻿using System;
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
            var version = Version;
            return await SuccessResponse(new { Version = version.Item1, PublishedDt = version.Item2 });
		}

        /// <summary>
        /// Restart Server
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotInGame")]
        [HttpGet("Admin/RestartServer")]
        public async Task<IActionResult> RestartServer([FromHeader]string authToken)
        {
            var user = UserContext.User;
            if (user.Id != EnergoServer.AdminId)
                return await ErrorResponse("Unauthorized");

            ServerContext.InitCurrentContext();
            return await SuccessResponse(true);
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

	}
}
