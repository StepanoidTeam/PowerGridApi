using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    [Route("api/[controller]")]
    public class MainController : BaseController
    {
        /// <summary>
        /// Get API version
        /// </summary>
        [HttpGet("Version")]
        public string GetVersion()
        {
            return Version;
        }

        [HttpGet("Login")]
        public MessageModel Login(string username, string userId = null)
        {
            var errMsg = string.Empty;
            userId = EnergoServer.Current.Login(username, out errMsg, userId);
            return FormatReturn(errMsg, userId);
        }

        [HttpGet("StartGame")]
        public MessageModel StartGame(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (player.GameRoomRef != null)
            {
                player.GameRoomRef.Init(out errMsg);
                player.GameRoomRef.GameBoardRef.Start();
            }
            return FormatReturn(errMsg);
        }

        [HttpGet("GameStatus")]
        public GameBoardModel GetGameStatus(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new GameBoardModel() { IsSuccess = false, Message = errMsg };
            if (player.GameRoomRef == null || player.GameRoomRef.GameBoardRef == null)
                return new GameBoardModel() { IsSuccess = false, Message = "Not in game" };
            return new GameBoardModel()
            {
                IsSuccess = true,
                GameBoard = (GameBoardViewModel)player.GameRoomRef.GameBoardRef.ToModel()
            };
        }

        [HttpGet("PlayerInfo")]
        public PlayerModel GetPlayerInfo(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new PlayerModel() { IsSuccess = false, Message = errMsg };
            return new PlayerModel()
            {
                IsSuccess = true,
                PlayerInfo = (PlayerViewModel)player.ToModel()
            };
        }


    }
}
