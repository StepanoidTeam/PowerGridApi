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
    /// Base controller class for any controller in API
    /// </summary>
    [EnableCors("CorsPolicy")]
    public abstract partial class BaseController : Controller
    {
        private static decimal _version = 0.04m;

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


        protected BaseController()
        {
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (await IfAuthorized(context, next))
            {
                if (await IfNotRestricted(context, next))
                    await base.OnActionExecutionAsync(context, next);
            }
        }
    }
}
