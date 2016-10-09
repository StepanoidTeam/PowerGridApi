using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;

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
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("AllowedActions/{userId}")]
        public ApiResponseModel GetAllowedActions(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (!player.IsInGame())
                return FormatReturn("Not in game");
            var lst = player.GameRoomRef.GameBoardRef.GetAllowedActions(userId, out errMsg);
            return FormatReturn(errMsg, lst);
        }

        /// <summary>
        /// Do specific game action
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost("DoAction/{userId}/{userAction}")]
        public ApiResponseModel DoAction(string userId, GameActionEnum userAction)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (!player.IsInGame())
                return FormatReturn("Not in game");
            var gbRef = player.GameRoomRef.GameBoardRef;
            switch (userAction)
            {
                case GameActionEnum.AuctionPass:
                    if (!gbRef.AuctionPass(userId, out errMsg))
                        return FormatReturn(errMsg);
                    return FormatReturn(null, true);
            }
            return FormatReturn("Incorrect action");
        }
    }
}
