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
    /// 
    /// </summary>
    public abstract partial class BaseController : Controller
    {   
        /// <summary>
        /// Restriction by user stage (in game / in room). Also will be added restriction by current game phase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private async Task<bool> IfNotRestricted(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var restrictAttr = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttribute<RestrictByStateAttribute>();
            if (restrictAttr != null)
            {
                switch(restrictAttr.UserState)
                {
                    case UserState.InGame:
                        if (!UserContext.User.IsInGame())
                        {
                            context.Result = await GenericResponse(ResponseType.NotInGame, Constants.Instance.ErrorMessage.YouAre_Not_In_Game);
                            return false;
                        }
                        break;

                    case UserState.InRoom:
                        if (!UserContext.User.IsInRoom())
                        {
                            context.Result = await GenericResponse(ResponseType.NotInRoom, Constants.Instance.ErrorMessage.YouAre_Outside_Of_Game_Rooms);
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

    }
}
