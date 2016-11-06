using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

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
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginInfo)
		{
            if(string.IsNullOrWhiteSpace(loginInfo.Username))
                return await GenericResponse(ResponseType.InvalidModel, "Username is required");

            var errMsg = string.Empty;
			var user = EnergoServer.Current.Login(loginInfo.Username, out errMsg);
            
            var result = await Task.Run(() =>
            {
                var obj = new UserModel(user).GetInfo(new UserModelViewOptions() { Id = true, Name = true });
                obj.Add("AuthToken", user.AuthToken);
                return obj;
            });

            return await GenericResponse(errMsg, result, ResponseType.InvalidModel);
		}

        /// <summary>
        /// Check if authorization token is not expired yet
        /// </summary>
        /// <returns></returns>
        [HttpPost("CheckAuthorization")]
        public async Task<IActionResult> CheckAuthorization([FromHeader]string authToken)
        {
            return await GenericResponse(ResponseType.Ok);
        }

        /// <summary>
        /// Log out user
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromHeader]string authToken)
        {
            var result = EnergoServer.Current.Logout(UserContext.User);
            return await GenericResponse(result ? ResponseType.Ok : ResponseType.UnexpectedError);
        }

	}
}
