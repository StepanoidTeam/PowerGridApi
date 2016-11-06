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
        [HttpGet]
        public async Task<IActionResult> GetGameRoomList([FromHeader]string authToken)
        {
            var rooms = EnergoServer.Current.GetGameRoomList(UserContext.User);
            var roomModels = rooms.Select(m => new GameRoomModel(m)).ToArray();

            var result = await Task.Run(() => 
            {
                //var usersViewOptions = new UserModelViewOptions { Name = true };
                return roomModels.Select(m => m.GetInfo(new RoomModelViewOptions
                    { Id = true, Name = true, IsInGame = true, UserCount = true }));
            });
            return await SuccessResponse(result);
        }

        /// <summary>
        /// Rooms list with filter and view options
        /// </summary>
        /// <param name="lwo">Lookup settings and view options</param>
        /// <returns></returns>
        [HttpPost("List")]
        public async Task<IActionResult> GetGameRoomList([FromHeader]string authToken, [FromBody]RoomsLookupWithOptions lwo)
        {
            var rooms = EnergoServer.Current.GetGameRoomList(UserContext.User, lwo.LookupSettings);

            var roomModels = rooms.Select(m => new GameRoomModel(m)).ToArray();
            var result = await Task.Run(() => { return roomModels.Select(m => m.GetInfo(lwo.Options)); });

            return await SuccessResponse(result);
        }

        /// <summary>
        /// Create Game Room 
        /// </summary>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateGameRoom([FromHeader]string authToken, [FromBody]CreateRoomModel room)
        {
            var errMsg = string.Empty;
            var gameRoom = EnergoServer.Current.CreateGameRoom(UserContext.User, room.Name, out errMsg);
            var result = await Task.Run(() =>
            {
                return new GameRoomModel(gameRoom).GetInfo(new RoomModelViewOptions { Id = true, Name = true });
            });

            return await GenericResponse(errMsg, result);
        }

        /// <summary>
        /// Join player into specific room
        /// </summary>
        /// <param name="joinModel"></param>
        /// <returns></returns>
        [HttpPost("Join")]
        public async Task<IActionResult> JoinGameRoom([FromHeader]string authToken, JoinRoomModel joinModel)
        {
            var errMsg = string.Empty;
            var gameRoom = EnergoServer.Current.LookupGameRoom(UserContext.User, joinModel.RoomId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);

            gameRoom.Join(UserContext.User, out errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);

            var result = await Task.Run(() =>
            {
                return new GameRoomModel(gameRoom).GetInfo(
                    new RoomModelViewOptions
                    {
                        Id = true, Name = true, UserCount = true, UserDetails = true,
                        UserViewOptions = new UserModelViewOptions { Id = true, Name = true, ReadyMark = true }
                    });
            });

            return await GenericResponse(errMsg, result);
        }

        /// <summary>
        /// Leave from current room
        /// </summary>
        /// <returns></returns>
        [RestrictByState(UserState.InRoom)]
        [HttpPost("Leave")]
        public async Task<IActionResult> LeaveGameRoom([FromHeader]string authToken)
        {
            var errMsg = string.Empty;

            var user = UserContext.User;
            //todo wtf again with UserContext.User....UserContext.User?
            user.GameRoomRef.Leave(user);

            if (user.GameRoomRef != null)
                return await GenericResponse(Constants.Instance.ErrorMessage.You_Cant_Leave_This_Game_Room);
            return await GenericResponse(errMsg);
        }

        /// <summary>
        /// Kick another player from the room if current user have enough permissions
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [RestrictByState(UserState.InRoom)]
        [HttpPost("Kick")]
        public async Task<IActionResult> Kick([FromHeader]string authToken, string username)
        {
            var errMsg = string.Empty;
            var leader = UserContext.User;
           
            var gameRoom = leader.GameRoomRef;

            var userId = gameRoom.Kick(leader, username, out errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            if (gameRoom.Players.ContainsKey(userId))
                errMsg = Constants.Instance.ErrorMessage.You_Cant_Kick_This_User;

            return await GenericResponse(errMsg);
        }

        /// <summary>
        /// Set if player ready to start or not
        /// </summary>
        /// <param name="state">in case it's not null - set ready mark to specified value, otherwise it will be toggled according to curent state</param>
        /// <returns></returns>
        [RestrictByState(UserState.InRoom)]
        [HttpPost("ToggleReady")]
        public async Task<IActionResult> SetReadyMarkTo([FromHeader]string authToken, bool? state = null)
        {
            var errMsg = string.Empty;
            var user = UserContext.User;
            bool result = false;

            if (state.HasValue)
                result = user.GameRoomRef.SetReadyMarkTo(user, state.Value, out errMsg);
            else
                result = user.GameRoomRef.ToogleReadyMark(user, out errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);
            return await GenericResponse(null, result);
        }

        /// <summary>
        /// Initiate game
        /// </summary>
        /// <returns></returns>
        [RestrictByState(UserState.InRoom)]
        [HttpGet("StartGame")]
        public async Task<IActionResult> StartGame([FromHeader]string authToken)
        {
            var errMsg = string.Empty;
            var user = UserContext.User;

            user.GameRoomRef.Init(out errMsg);
            user.GameRoomRef.GameBoardRef.Start();
            
            return await GenericResponse(errMsg);
        }

    }
}
