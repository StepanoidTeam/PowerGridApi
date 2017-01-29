using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.SwaggerGen.Annotations;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
	/// <summary>
	/// Registration, Login, Logoff, Recover password, etc.
	/// </summary>
	[Route("api/[controller]")]
	public class UserController : BaseController
    {
        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
		[AllowAnonymous]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "InvalidModel")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginInfo)
        {
            if (string.IsNullOrWhiteSpace(loginInfo.Username))
                return await GenericResponse(ResponseType.InvalidModel, "Username is required");

            var errMsg = string.Empty;
            var user = EnergoServer.Current.Login(loginInfo.Username, out errMsg);
            
            return await GenericResponse(() =>
            {
                ServerContext.Current.Chat.AddChannel(user, ChatChannelType.Private, user.Id);

                var broadcast = new UserModel(user).GetInfo(new UserModelViewOptions()
                {
                    Id = true,
                    Name = true
                }).AddItem(BroadcastReason, Request.Path.Value);

                ServerContext.Current.DuplexNetwork.Broadcast(broadcast);

                var obj = new UserModel(user).GetInfo(new UserModelViewOptions() { Id = true, Name = true });
                obj.Add("AuthToken", user.AuthToken);
                return obj;
            }, errMsg, ResponseType.InvalidModel).Invoke();
        }

        /// <summary>
        /// Log out user
        /// </summary>
        /// <returns>Possible statuses: Unauthorized, Ok</returns>
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromHeader]string authToken)
        {
            ApiResponseModel result = null;
            await Task.Run(() => { result = ServerContext.Current.UserModule.Logout(UserContext.User); });
            return await GenericResponse(result);
        }

        /// <summary>
        /// Get Player info. It also could be good enter point to check if Auth Token is not expired yet.
        /// </summary>
        /// <returns></returns>        
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Unauthorized")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [HttpPost("Status")]
        public async Task<IActionResult> GetUserInfo([FromHeader]string authToken, [FromBody]UserModelViewOptions viewOptions)
        {
            var responseGetter = SuccessResponse(() =>
            {
                return ServerContext.Current.UserModule.GetStatus(UserContext.User, viewOptions);
            });

            return await responseGetter();
        }
    }
}
