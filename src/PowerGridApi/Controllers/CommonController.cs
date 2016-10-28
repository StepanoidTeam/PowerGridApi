using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

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
        /// Get status of game if it's active for current user, otherwise it will return appopriate message
        /// </summary>
        /// <returns></returns>
		[HttpGet("Status/Game")]
		public async Task<IActionResult> GetGameStatus([FromHeader]string authToken)
		{
			var errMsg = string.Empty;
			var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return await GenericResponse(errMsg);
			if (player.GameRoomRef == null || player.GameRoomRef.GameBoardRef == null)
				return await GenericResponse("Not in game");

			var gameBoardModel = new GameBoardModel(player.GameRoomRef.GameBoardRef);
            var result = await Task.Run(() => { return gameBoardModel.GetInfo(); });

            return await SuccessResponse(result);
		}

        /// <summary>
        /// Player info
        /// </summary>
        /// <returns></returns>
		[HttpGet("Status/Player")]
		public async Task<IActionResult> GetPlayerInfo([FromHeader]string authToken)
		{
			var errMsg = string.Empty;
			var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return await GenericResponse(errMsg);

			var playerModel = new UserModel(player);
            var result = await Task.Run(() => { return playerModel.GetInfo(); });

            return await SuccessResponse(result);
		}

	}
}
