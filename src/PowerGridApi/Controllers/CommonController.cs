﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
	/// <summary>
	/// Various additional methods
	/// </summary>
	/// 
	[EnableCors("CorsPolicy")]
	[Route("api/")]
	public class CommonController : BaseController
    {
        /// <summary>
        /// Get API version
        /// </summary>
        [HttpGet("Version")]
		public async Task<ApiResponseModel> GetVersion()
		{
            return await SuccessResponse(Version);
		}

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
		[HttpPost("Login/{username}")]
        public async Task<ApiResponseModel> Login(string username, string userId = null)
		{
			var errMsg = string.Empty;
			userId = EnergoServer.Current.Login(username, out errMsg, userId);
			return await GenericResponse(errMsg, userId);
		}

        /// <summary>
        /// Get status of game if it's active for current user, otherwise it will return appopriate message
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
		[HttpGet("Status/Game/{userId}")]
		public async Task<ApiResponseModel> GetGameStatus(string userId)
		{
			var errMsg = string.Empty;
			var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return await GenericResponse(errMsg);
			if (player.GameRoomRef == null || player.GameRoomRef.GameBoardRef == null)
				return await GenericResponse("Not in game");
			var gameBoardModel = new GameBoardModel(player.GameRoomRef.GameBoardRef);
			return await SuccessResponse(gameBoardModel.GetInfo());
		}

        /// <summary>
        /// Player info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
		[HttpGet("Status/Player/{userId}")]
		public async Task<ApiResponseModel> GetPlayerInfo(string userId)
		{
			var errMsg = string.Empty;
			var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return await GenericResponse(errMsg);
			var playerModel = new PlayerModel(player);
			return await SuccessResponse(playerModel.GetInfo());
		}

	}
}
