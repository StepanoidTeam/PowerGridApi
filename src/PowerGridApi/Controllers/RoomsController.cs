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
    /// Create your room or join to some existing one
    /// </summary>
    [Route("api/[controller]")]
    public class RoomsController : BaseController
    {
        /// <summary>
        /// Rooms list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<ApiResponseModel> GetGameRoomList(string userId)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            var roomModels = rooms.Select(m => new GameRoomModel(m)).ToArray();

            var result = await Task.Run(() => { return roomModels.Select(m => m.GetInfo()); });
            return await SuccessResponse(result);
        }
        
        [HttpPost("Create/{userId}/{name}")]
        public async Task<ApiResponseModel> CreateGameRoom(string userId, string name)
        {
            var errMsg = string.Empty;
            var gameRoomId = EnergoServer.Current.CreateGameRoom(userId, name, out errMsg);
            return await GenericResponse(errMsg, gameRoomId);
        }
       
        [HttpPost("List")]
        public async Task<ApiResponseModel> GetGameRoomList(string userId, RoomModelViewOptions options = null, RoomLookupSettings lookupSettings = null)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(userId, out errMsg, lookupSettings);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);

            var roomModels = rooms.Select(m => new GameRoomModel(m)).ToArray();
            var result = await Task.Run(() => { return roomModels.Select(m => m.GetInfo(options)); });

            return await SuccessResponse(result);
        }

        /// <summary>
        /// Join player into specific room
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameRoomId"></param>
        /// <returns></returns>
        [HttpPost("Join")]
        public async Task<ApiResponseModel> JoinGameRoom(string userId, string gameRoomId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            var gameRoom = EnergoServer.Current.LookupGameRoom(userId, gameRoomId, out errMsg);
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
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("Leave")]
        public async Task<ApiResponseModel> LeaveGameRoom(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (player.GameRoomRef == null)
                return await GenericResponse(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);

            player.GameRoomRef.Leave(userId);

            if (player.GameRoomRef != null)
                return await GenericResponse(Constants.Instance.CONST_ERR_MSG_YOU_CANT_LEAVE_THIS_GAME_ROOM);
            return await GenericResponse(errMsg);
        }

        /// <summary>
        /// Kick another player from the room if current user have enough permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost("Kick")]
        public async Task<ApiResponseModel> Kick(string userId, string username)
        {
            var errMsg = string.Empty;
            var leader = EnergoServer.Current.LookupPlayer(userId, out errMsg);
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
        /// <param name="userId"></param>
        /// <param name="state">in case it's not null - set ready mark to specified value, otherwise it will be toogled according to curent state</param>
        /// <returns></returns>
        [HttpPost("ToogleReady")]
        public async Task<ApiResponseModel> SetReadyMarkTo(string userId, bool? state = null)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
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
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("StartGame")]
        public async Task<ApiResponseModel> StartGame(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
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
