using System;
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
		public ApiResponseModel GetVersion()
		{
			return FormatSuccessReturn(Version);
		}

		[HttpPost("Login/{username}")]
		public ApiResponseModel Login(string username, string userId = null)
		{
			var errMsg = string.Empty;
			userId = EnergoServer.Current.Login(username, out errMsg, userId);
			return FormatReturn(errMsg, userId);
		}

		[HttpGet("Status/Game/{userId}")]
		public ApiResponseModel GetGameStatus(string userId)
		{
			var errMsg = string.Empty;
			var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return FormatReturn(errMsg);
			if (player.GameRoomRef == null || player.GameRoomRef.GameBoardRef == null)
				return FormatReturn("Not in game");
			var gameBoardModel = new GameBoardModel(player.GameRoomRef.GameBoardRef);
			return FormatSuccessReturn(gameBoardModel.GetInfo());
		}

		[HttpGet("Status/Player/{userId}")]
		public ApiResponseModel GetPlayerInfo(string userId)
		{
			var errMsg = string.Empty;
			var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return FormatReturn(errMsg);
			var playerModel = new PlayerModel(player);
			return FormatSuccessReturn(playerModel.GetInfo());
		}


	}
}
