using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    public enum ResponseType
    {
        Ok,
        Error,
        Unauthorized
    }

    /// <summary>
    /// Base controller class for any controller in API
    /// </summary>
    [EnableCors("CorsPolicy")]
    public abstract class BaseController : Controller
    {
        private static decimal _version = 0.02m;

        /// <summary>
        /// Version of current API
        /// </summary>
        public static string Version
        {
            get
            {
                return string.Format("v{0}", _version.ToString(CultureInfo.InvariantCulture));
            }
        }

        public string UserId
        {
            get
            {
                var identity = HttpContext.User.Identities.FirstOrDefault(m => m.AuthenticationType == "custom");
                if (identity == null || !identity.IsAuthenticated)
                    return "";
                return identity.Name;
            }
        }

        /// <summary>
        /// If response is for sure successfull
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task<IActionResult> SuccessResponse(object data)
        {
            return await Task.Run(() =>
            {
                return Ok(data);
            });
        }

        protected async Task<IActionResult> ErrorResponse(string errMsg)
        {
            return await Task.Run(() =>
            {
                return base.Json(new ApiResponseModel(errMsg, false));
            });
        }

        /// <summary>
        /// Use this type of reponse in case you need to check if it's ok (errMsg is empty) and return data,
        /// or errMsg is not empty and need to return it as an Error response
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task<IActionResult> GenericResponse(string errMsg, object data = null)
        {
            if (string.IsNullOrWhiteSpace(errMsg))
                return await SuccessResponse(data);
            return await ErrorResponse(errMsg);
        }

        protected async Task<IActionResult> GenericResponse(ResponseType type, string errMsg = null, object data = null)
        {
            switch(type)
            {
                case ResponseType.Ok:
                    return await SuccessResponse(data);
                case ResponseType.Error:
                    return await ErrorResponse(errMsg ?? "Unexpected error");
                case ResponseType.Unauthorized:
                    return Unauthorized();
                default:
                    return NotFound();
            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var allowAnonym = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>();
            if (allowAnonym == null)
            {
                var authHeaderKey = "authToken";
                var authHeader = "";
                if (context.HttpContext.Request.Headers.ContainsKey(authHeaderKey))
                    authHeader = context.HttpContext.Request.Headers[authHeaderKey].ToString();

                var errMsg = "";
                var player = EnergoServer.Current.LookupPlayer(authHeader, out errMsg);
                if (player == null)
                {
                    context.Result = Unauthorized();
                    return;
                }
                else
                {
                    var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, player.Id) };
                    var id = new ClaimsIdentity(claims, "custom", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    context.HttpContext.User.AddIdentity(id);
                }              
            }

            await base.OnActionExecutionAsync(context, next);
        }

    }
}
