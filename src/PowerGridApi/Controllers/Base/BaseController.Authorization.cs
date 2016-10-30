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
    /// <summary>
    /// Authorization part of Base controller class for any controller in API
    /// </summary>
    public abstract partial class BaseController : Controller
    {
        public UserContext CurrentUser { get; private set; }
        
        /// <summary>
        /// Doing manual custom authorization for EACH method WITHOUT AllowAnonymousAttribute.
        /// Authorization is ok in case header with key 'authToken' contains user authorization token previously returned by
        /// EnergoServer Login method and was not expired yet.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var allowAnonym = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>();
            if (allowAnonym == null)
            {
                var authHeaderKey = "authToken";
                var authHeader = "";
                if (context.HttpContext.Request.Headers.ContainsKey(authHeaderKey))
                    authHeader = context.HttpContext.Request.Headers[authHeaderKey].ToString();

                string errMsg = "";
                var user = EnergoServer.Current.FindUserByAuthToken(authHeader, out errMsg);
                if (user == null)
                {
                    context.Result = await GenericResponse(ResponseType.Unauthorized);
                    return;
                }
                else
                {
                    var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id) };
                    var id = new ClaimsIdentity(claims, "custom", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    context.HttpContext.User.AddIdentity(id);

                    CurrentUser = new UserContext(user);
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }

    }
}
