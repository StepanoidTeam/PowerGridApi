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

        /// <summary>
        /// Get list of active chat channels or chat channels where you have invite to
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "InvalidModel")]
        [HttpGet("Channels")]
        public async Task<IActionResult> ChannelsList([FromHeader]string authToken)
        {
            var channels = ServerContext.Current.Chat.GetUserChannels(UserContext.User, true);

            return await SuccessResponse(() =>
            {
                return new ApiResponseModel(channels.Select(channel => new ChannelModel(channel.Key, channel.Value).GetInfo(
                    new ChannelModelViewOptions(true))));
            }).Invoke();
        }

        /// <summary>
        /// Create custom channel
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="model">only Name is used</param>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "InvalidModel")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateChannel([FromHeader]string authToken, [FromBody]ChannelRequestModel model)
        {
            var errMsg = string.Empty;
            try
            {
                var channel = ServerContext.Current.Chat.AddChannel(UserContext.User, ChatChannelType.Custom, name: model.Name);

                return await SuccessResponse(new ChannelModel(channel, true)
                    .GetInfo(new ChannelModelViewOptions(true)));
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return await ErrorResponse(errMsg, ResponseType.InvalidModel);
        }

        /// <summary>
        /// Could be closed only Custom channels
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="model">only Id is used</param>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "InvalidModel")]
        [HttpPost("Close")]
        public async Task<IActionResult> CloseChannel([FromHeader]string authToken, [FromBody]ChannelRequestModel model)
        {
            var channel = ServerContext.Current.Chat.LookupChannel(model.Id, ChatChannelType.Custom);
            if (channel == null)
                return await GenericResponse(ResponseType.InvalidModel, string.Format(ChatNetworkModule.ErrMsg_NoSuchChannelOrNotAllow, "close it"));

            var errMsg = string.Empty;
            try
            {
                ServerContext.Current.Chat.DropChannel(UserContext.User, channel);
                return await SuccessResponse(true);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return await ErrorResponse(errMsg, ResponseType.InvalidModel);
        }

        /// <summary>
        /// Join custom channel
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="model">only Id is used</param>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "InvalidModel")]
        [HttpPost("Join")]
        public async Task<IActionResult> JoinChannel([FromHeader]string authToken, [FromBody]ChannelRequestModel model)
        {
            var errMsg = string.Empty;
            try
            {
                var channel = ServerContext.Current.Chat.Join(UserContext.User, model.Id);
                return await SuccessResponse(channel);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return await ErrorResponse(errMsg, ResponseType.InvalidModel);
        }

        /// <summary>
        /// Leave custom channel
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="model">only Id is used</param>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "InvalidModel")]
        [HttpPost("Leave")]
        public async Task<IActionResult> LeaveChannel([FromHeader]string authToken, [FromBody]ChannelRequestModel model)
        {
            var errMsg = string.Empty;
            try
            {
                var channel = ServerContext.Current.Chat.Leave(UserContext.User, model.Id);
                return await SuccessResponse(channel);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return await ErrorResponse(errMsg, ResponseType.InvalidModel);
        }

        /// <summary>
        /// Put user into channel whitelist (if whitelist is enabled) and send him invite to join channel
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "InvalidModel")]
        [HttpPost("Invite")]
        public async Task<IActionResult> InviteToChannel([FromHeader]string authToken, [FromBody]InviteToChannelModel model)
        {
            var errMsg = string.Empty;
            try
            {
                ServerContext.Current.Chat.InviteToChannel(UserContext.User, model.UserId, model.ChannelId);
                return await SuccessResponse(true);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return await ErrorResponse(errMsg, ResponseType.InvalidModel);
        }
    }
}
