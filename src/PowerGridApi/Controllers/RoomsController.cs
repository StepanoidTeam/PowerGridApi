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
    public class RoomsController : BaseController
    {
        [HttpGet("")]
        public GameRoomsModel GetGameRoomList(string userId)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(userId, out errMsg, null);
            var rvm = new List<GameRoomViewModel>();
            foreach (var r in rooms)
                rvm.Add((GameRoomViewModel)r.ToViewModel());
            return new GameRoomsModel()
            {
                GameRooms = rvm.ToArray(),
                Message = errMsg,
                IsSuccess = string.IsNullOrWhiteSpace(errMsg)
            };
        }

        [HttpGet("Create")] //it's just for test purposes, for production we should use POSTs in case create data
        [HttpGet("Create/{userId}/{name}")] //it's just for test purposes, for production we should use POSTs in case create data
        [HttpPost("Create")]
        public MessageModel CreateGameRoom(string userId, string name)
        {
            var errMsg = string.Empty;
            var gameRoomId = EnergoServer.Current.CreateGameRoom(userId, name, out errMsg);
            return FormatReturn(errMsg, gameRoomId);
        }
       
        [HttpPost("List")]
        public GameRoomsModel GetGameRoomList(string userId, RoomsViewModelOptions options = null, RoomLookupSettings lookupSettings = null)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(userId, out errMsg, lookupSettings);
            var rvm = new List<GameRoomViewModel>();
            foreach (var r in rooms)
                rvm.Add((GameRoomViewModel)r.ToViewModel(options));
            return new GameRoomsModel()
            {
                GameRooms = rvm.ToArray(),
                Message = errMsg,
                IsSuccess = string.IsNullOrWhiteSpace(errMsg)
            };
        }

        [HttpGet("Join")]
        public MessageModel JoinGameRoom(string userId, string gameRoomId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            var gameRoom = EnergoServer.Current.LookupGameRoom(userId, gameRoomId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            gameRoom.Join(player, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(errMsg, gameRoom.Id);
        }

        [HttpGet("Leave")]
        public MessageModel LeaveGameRoom(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (player.GameRoomRef == null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);
            player.GameRoomRef.Leave(userId);
            if (player.GameRoomRef != null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOU_CANT_LEAVE_THIS_GAME_ROOM);
            return FormatReturn(errMsg);
        }

        [HttpGet("Kick")]
        public MessageModel Kick(string userId, string username)
        {
            var errMsg = string.Empty;
            var leader = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (leader.GameRoomRef == null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);
            var gameRoom = leader.GameRoomRef;
            var playerId = gameRoom.Kick(leader.Id, username, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (gameRoom.Players.ContainsKey(playerId))
                errMsg = Constants.Instance.CONST_ERR_MSG_YOU_CANT_KICK_THIS_USER;

            return FormatReturn(errMsg);
        }

        [HttpGet("SetReadyMark")]
        public MessageModel SetReadyMarkTo(string userId, bool state)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            var ret = player.GameRoomRef.SetReadyMarkTo(player, state, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(null, ret);
        }

    }
}
