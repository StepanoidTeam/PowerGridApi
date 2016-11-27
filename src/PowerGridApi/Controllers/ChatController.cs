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
        public async Task<IActionResult> ReceiveMessage([FromHeader]string authToken, [FromBody]ChatSendModel message)
        {
            ApiResponseModel result = null;
            await Task.Run(() => { result = ServerContext.Current.Chat.ReceiveMessage(UserContext.User, message); });
            return await GenericResponse(result);
        }
        
	}
}
