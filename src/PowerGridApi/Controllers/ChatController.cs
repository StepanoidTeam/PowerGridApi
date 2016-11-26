using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using Microsoft.AspNetCore.Cors;
using Swashbuckle.SwaggerGen.Annotations;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    /// <summary>
    /// Send messages to other players
    /// </summary>
	[Route("api/[controller]")]
	public class ChatController : BaseController
    {
        /// <summary>
        /// Send message to chat
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
		[SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "NotAllowed")]
        [HttpPost("Send")]
		public async Task<IActionResult> CreateGameRoom([FromHeader]string authToken, [FromBody]ChatSendModel message)
		{
			if (message.InRoomChannel && !UserContext.User.IsInRoom())
				return await GenericResponse(ResponseType.NotAllowed, Constants.Instance.ErrorMessage.Not_In_Room);

            string reveivers = null;
            if (message.InRoomChannel)
                reveivers = UserContext.User.GameRoomRef.Id;
            else if (!string.IsNullOrWhiteSpace(message.ToUserId))
                reveivers = message.ToUserId;

            WebSocketManager.Current.Broadcast(message, reveivers);

            return await SuccessResponse(true);
		}
        
	}
}
