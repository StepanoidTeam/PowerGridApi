using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using Swashbuckle.SwaggerGen.Annotations;

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
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
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
		[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
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
		/// <param name="room"></param>
		/// <returns></returns>
		[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
		[SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotAllowed")]
		[HttpPost("Create")]
		public async Task<IActionResult> CreateGameRoom([FromHeader]string authToken, [FromBody]CreateRoomModel room)
		{
			var errMsg = string.Empty;
			if (UserContext.User.IsInRoom())
				return await GenericResponse(ResponseType.NotAllowed, Constants.Instance.ErrorMessage.Is_In_Game_Room);

			var gameRoom = EnergoServer.Current.CreateGameRoom(UserContext.User, room.Name, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return await GenericResponse(ResponseType.InvalidModel, errMsg);

			ToggleReadyResponse response = null;
			if (room.SetReadyMark)
				response = EnergoServer.Current.RouteAction(new ToggleReadyAction(UserContext.User));

			return await SuccessResponse(() =>
			{
                ServerContext.Current.Chat.AddChannel(UserContext.User, ChatChannelType.Room, gameRoom.Id);

				var broadcast = new GameRoomModel(gameRoom).GetInfo(new RoomModelViewOptions(true)
				{
					UserViewOptions = new UserModelViewOptions(false)
				}).AddItem(BroadcastReason, Request.Path.Value);

                WebSocketManager.Current.Broadcast(broadcast);

                return new GameRoomModel(gameRoom).GetInfo(new RoomModelViewOptions(true)
                {
                    IsInGame = false,
                    UserCount = false,
                    UserViewOptions = new UserModelViewOptions(false) { ReadyMark = true }
                });
            }).Invoke();
		}

		/// <summary>
		/// Join player into specific room
		/// </summary>
		/// <param name="joinModel"></param>
		/// <returns></returns> 
		[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
		[SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotAllowed")]
		[SwaggerResponse(System.Net.HttpStatusCode.NotFound, "NotFound")]
		[HttpPost("Join")]
		public async Task<IActionResult> JoinGameRoom([FromHeader]string authToken, [FromBody]JoinRoomModel joinModel)
		{
			var errMsg = string.Empty;
			var gameRoom = EnergoServer.Current.LookupGameRoom(UserContext.User, joinModel.RoomId, out errMsg);
			if (!string.IsNullOrWhiteSpace(errMsg))
				return await GenericResponse(ResponseType.NotFound, errMsg);

			gameRoom.Join(UserContext.User, out errMsg);

			if (!string.IsNullOrWhiteSpace(errMsg))
				return await GenericResponse(ResponseType.NotAllowed, errMsg);

			return await SuccessResponse(() =>
			{
				var broadcast = new GameRoomModel(gameRoom).GetInfo(new RoomModelViewOptions()
				{
					UserCount = true,
					UserViewOptions = new UserModelViewOptions(false) { Id = true}
				}).AddItem(BroadcastReason, Request.Path.Value);

				WebSocketManager.Current.Broadcast(broadcast, gameRoom.Id);

				return new GameRoomModel(gameRoom).GetInfo(
					new RoomModelViewOptions
					{
						Id = true,
						Name = true,
						UserCount = true,
						UserDetails = true,
						UserViewOptions = new UserModelViewOptions { Id = true, Name = true, ReadyMark = true }
					});
			}).Invoke();
		}

		/// <summary>
		/// Leave from current room
		/// </summary>
		/// <returns></returns>
		[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
		[RestrictByState(UserState.InRoom)]
		[HttpPost("Leave")]
		public async Task<IActionResult> LeaveGameRoom([FromHeader]string authToken)
		{
			var errMsg = string.Empty;

			var user = UserContext.User;
			var room = user.GameRoomRef;
            var roomId = room.Id;

			//todo wtf again with UserContext.User....UserContext.User?
			user.GameRoomRef.Leave(user);

			if (user.GameRoomRef != null)
				return await GenericResponse(ResponseType.UnexpectedError, Constants.Instance.ErrorMessage.You_Cant_Leave_This_Game_Room);

            if (user.GameRoomRef != null && room.Players.Count == 0)
                ServerContext.Current.Chat.DropChannel(user, roomId);

			var broadcast = new GameRoomModel(room).GetInfo(new RoomModelViewOptions()
			{
				UserCount = true,
				UserViewOptions = new UserModelViewOptions(false) { Id = true }
			}).AddItem(BroadcastReason, Request.Path.Value);

			WebSocketManager.Current.Broadcast(broadcast, room.Id);

			//todo why it is possible you couldn't leave room? Need to determine corrent status code here
			return await SuccessResponse(true);
		}

		/// <summary>
		/// Kick another player from the room if current user have enough permissions
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
		[SwaggerResponse(System.Net.HttpStatusCode.NotFound, "NotFound")]
		[SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotInRoom, NotAllowed")]
		[RestrictByState(UserState.InRoom)]
		[HttpPost("Kick")]
		public async Task<IActionResult> Kick([FromHeader]string authToken, string username)
		{
			var errMsg = string.Empty;
			var leader = UserContext.User;

			var gameRoom = leader.GameRoomRef;

			var user = ServerContext.Current.Server.LookupUserByName(username);
			//Allow to kick player even In Game.. Possible todo don't kick instantly, but only in case All players will agree 
			//with this or He will agree himself or even leave
			if (user == null || user.GameRoomRef == null || leader.GameRoomRef == null || user.GameRoomRef.Id != leader.GameRoomRef.Id)
				return await ErrorResponse(Constants.Instance.ErrorMessage.There_No_Such_User, ResponseType.NotFound);

			var userId = gameRoom.Kick(leader, user, out errMsg);

			if (!string.IsNullOrWhiteSpace(errMsg))
				//todo find what is reason for error
				return await GenericResponse(ResponseType.NotAllowed, errMsg);

			if (gameRoom.Players.ContainsKey(userId))
				errMsg = Constants.Instance.ErrorMessage.You_Cant_Kick_This_User;

            var broadcast = new GameRoomModel(gameRoom).GetInfo(new RoomModelViewOptions()
            {
                UserCount = true,
                UserViewOptions = new UserModelViewOptions(false) { Id = true }
            }).AddItem(BroadcastReason, Request.Path.Value);

            WebSocketManager.Current.Broadcast(broadcast, gameRoom.Id);

            var broadcastToKicked = new UserModel(user).GetInfo(new UserModelViewOptions() { Id = true, GameRoomId = true })
                .AddItem(BroadcastReason, Request.Path.Value); ;
            
            WebSocketManager.Current.Broadcast(broadcastToKicked, user.Id);

            return await GenericResponse(ResponseType.UnexpectedError, errMsg);
		}

		/// <summary>
		/// Set if player ready to start or not
		/// </summary>
		/// <param name="state">in case it's not null - set ready mark to specified value, otherwise it will be toggled according to curent state</param>
		/// <returns></returns>
		[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
		[SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotInRoom")]
		[RestrictByState(UserState.InRoom)]
		[HttpPost("ToggleReady")]
		public async Task<IActionResult> SetReadyMarkTo([FromHeader]string authToken, [FromBody]ToggleReadyModel toggleModel)
		{
			var user = UserContext.User;

			var toggleResponse = EnergoServer.Current.RouteAction(new ToggleReadyAction(user, toggleModel.State));

			//there are couldn't be actually errors, because current method is expecting you are in game and we user
			//YOUR (user) game for sure. So only possible is Unexpected error
			if (!toggleResponse.IsSuccess)
				return await GenericResponse(ResponseType.UnexpectedError, toggleResponse.ErrorMsg);

            var room = user.GameRoomRef;
            var broadcast = new GameRoomModel(room).GetInfo(new RoomModelViewOptions()
            {
                UserViewOptions = new UserModelViewOptions(false) { ReadyMark = true, Id = true }
            }).AddItem(BroadcastReason, Request.Path.Value);

            WebSocketManager.Current.Broadcast(broadcast, room.Id);
            
			return await SuccessResponse(toggleResponse.CurrentState);
		}

		/// <summary>
		/// Initiate game
		/// </summary>
		/// <returns></returns>
		[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
		[SwaggerResponse(System.Net.HttpStatusCode.NotFound, "NotFound")]
		[RestrictByState(UserState.InRoom)]
		[HttpGet("StartGame")]
		public async Task<IActionResult> StartGame([FromHeader]string authToken)
		{
			var user = UserContext.User;

            //todo start game only if creator

            var startResponse = EnergoServer.Current.RouteAction(new StartGameAction(user));
            if(!startResponse.IsSuccess)
                return await GenericResponse(ResponseType.NotAllowed, errMsg: startResponse.ErrorMsg);

            var broadcast = new GameRoomModel(user.GameRoomRef).GetInfo(new RoomModelViewOptions()
            {
                Id = true,
                IsInGame = true,
                UserCount = true,
                UserViewOptions = new UserModelViewOptions(false)
            }).AddItem(BroadcastReason, Request.Path.Value);

            WebSocketManager.Current.Broadcast(broadcast);

            return await GenericResponse(ResponseType.Ok, data: startResponse);
		}

	}
}
