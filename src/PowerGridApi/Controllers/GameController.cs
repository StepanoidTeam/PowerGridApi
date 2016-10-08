using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    [Route("api/[controller]")]
    public class GameController : BaseController
    {
        //IN GAME

        [HttpGet("AllowedActions")]
        public MessageModel GetAllowedActions(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (!player.IsInGame())
                return FormatReturn("Not in game");
            var lst = player.GameRoomRef.GameBoardRef.GetAllowedActions(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(null, lst);
        }

        [HttpGet("DoAction")]
        public MessageModel DoAction(string userId, GameActionEnum action)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (!player.IsInGame())
                return FormatReturn("Not in game");
            var gbRef = player.GameRoomRef.GameBoardRef;
            switch (action)
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
