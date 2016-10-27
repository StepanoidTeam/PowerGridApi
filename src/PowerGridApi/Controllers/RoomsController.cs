using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    /// <summary>
    /// Create your room or join to some existing one
    /// </summary>
    [Route("api/[controller]")]
    public class RoomsController : BaseController
    {
        /// <summary>
        /// Rooms list
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> GetGameRoomList([FromHeader]string authToken)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            var roomModels = rooms.Select(m => new GameRoomModel(m)).ToArray();

            var result = await Task.Run(() => { return roomModels.Select(m => m.GetInfo()); });
            return await SuccessResponse(result);
        }
        
        [HttpPost("Create/{name}")]
        public async Task<IActionResult> CreateGameRoom([FromHeader]string authToken, string name)
        {
            var errMsg = string.Empty;
            var gameRoomId = EnergoServer.Current.CreateGameRoom(authToken, name, out errMsg);
            return await GenericResponse(errMsg, gameRoomId);
        }
       
        [HttpPost("List")]
        public async Task<IActionResult> GetGameRoomList([FromHeader]string authToken, RoomModelViewOptions options = null, RoomLookupSettings lookupSettings = null)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(authToken, out errMsg, lookupSettings);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);

            var roomModels = rooms.Select(m => new GameRoomModel(m)).ToArray();
            var result = await Task.Run(() => { return roomModels.Select(m => m.GetInfo(options)); });

            return await SuccessResponse(result);
        }

        /// <summary>
        /// Join player into specific room
        /// </summary>
        /// <param name="gameRoomId"></param>
        /// <returns></returns>
        [HttpPost("Join")]
        public async Task<IActionResult> JoinGameRoom([FromHeader]string authToken, string gameRoomId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            var gameRoom = EnergoServer.Current.LookupGameRoom(authToken, gameRoomId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);

            gameRoom.Join(player, out errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            return await GenericResponse(errMsg, gameRoom.Id);
        }

        /// <summary>
        /// Leave from current room
        /// </summary>
        /// <returns></returns>
        [HttpPost("Leave")]
        public async Task<IActionResult> LeaveGameRoom([FromHeader]string authToken)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (player.GameRoomRef == null)
                return await GenericResponse(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);

            player.GameRoomRef.Leave(authToken);

            if (player.GameRoomRef != null)
                return await GenericResponse(Constants.Instance.CONST_ERR_MSG_YOU_CANT_LEAVE_THIS_GAME_ROOM);
            return await GenericResponse(errMsg);
        }

        /// <summary>
        /// Kick another player from the room if current user have enough permissions
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost("Kick")]
        public async Task<IActionResult> Kick([FromHeader]string authToken, string username)
        {
            var errMsg = string.Empty;
            var leader = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (leader.GameRoomRef == null)
                return await GenericResponse(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);
            var gameRoom = leader.GameRoomRef;

            var playerId = gameRoom.Kick(leader.Id, username, out errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (gameRoom.Players.ContainsKey(playerId))
                errMsg = Constants.Instance.CONST_ERR_MSG_YOU_CANT_KICK_THIS_USER;

            return await GenericResponse(errMsg);
        }

        /// <summary>
        /// Set if player ready to start or not
        /// </summary>
        /// <param name="state">in case it's not null - set ready mark to specified value, otherwise it will be toggled according to curent state</param>
        /// <returns></returns>
        [HttpPost("ToggleReady")]
        public async Task<IActionResult> SetReadyMarkTo([FromHeader]string authToken, bool? state = null)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            bool result = false;

            if (state.HasValue)
                player.GameRoomRef.SetReadyMarkTo(player, state.Value, out errMsg);
            else
                player.GameRoomRef.ToogleReadyMark(player, out errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            return await GenericResponse(null, result);
        }

        /// <summary>
        /// Initiate game
        /// </summary>
        /// <returns></returns>
        [HttpGet("StartGame")]
        public async Task<IActionResult> StartGame([FromHeader]string authToken)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(authToken, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (player.GameRoomRef != null)
            {
                player.GameRoomRef.Init(out errMsg);
                player.GameRoomRef.GameBoardRef.Start();
            }
            return await GenericResponse(errMsg);
        }

    }
}
