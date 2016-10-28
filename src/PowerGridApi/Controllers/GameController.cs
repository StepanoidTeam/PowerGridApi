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
        /// <returns></returns>
        [HttpGet("AllowedActions")]
        public async Task<IActionResult> GetAllowedActions([FromHeader]string authToken)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (!player.IsInGame())
                return await GenericResponse("Not in game");

            var lst = player.GameRoomRef.GameBoardRef.GetAllowedActions(authToken, out errMsg);
            return await GenericResponse(errMsg, lst);
        }

        /// <summary>
        /// Do specific game action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost("DoAction")]
        public async Task<IActionResult> DoAction([FromHeader]string authToken, DoActionModel action)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (!player.IsInGame())
                return await GenericResponse("Not in game");
            var gbRef = player.GameRoomRef.GameBoardRef;
            switch (action.Action)
            {
                case GameActionEnum.AuctionPass:
                    if (!gbRef.AuctionPass(authToken, out errMsg))
                        return await GenericResponse(errMsg);
                    return await GenericResponse(null, true);
            }
            return await GenericResponse("Incorrect action");
        }
    }
}
