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
        [RestrictByState(UserState.InGame)]
        public async Task<IActionResult> GetAllowedActions([FromHeader]string authToken)
        {
            var errMsg = string.Empty;
            //todo wtf UserContext.User....(AuthToken???...)
            var lst = UserContext.User.GameRoomRef.GameBoardRef.GetAllowedActions(UserContext.User.AuthToken, out errMsg);
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
            return await GenericResponse("Incorrect action");
        }
    }
}
