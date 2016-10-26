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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    /// <summary>
    /// Base controller class for any controller in API
    /// </summary>
    [EnableCors("CorsPolicy")]
    public abstract class BaseController : Controller
    {
        private static decimal _version = 0.01m;

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

        /// <summary>
        /// If response is for sure successfull
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task<ApiResponseModel> SuccessResponse(object data)
        {
            return await Task.Run(() =>
            {
                return new ApiResponseModel(data);
            });
        }

        /// <summary>
        /// Use this type of reponse in case you need to check if it's ok (errMsg is empty) and return data,
        /// or errMsg is not empty and need to return it as an Error response
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task<ApiResponseModel> GenericResponse(string errMsg, object data = null)
        {
            if (string.IsNullOrWhiteSpace(errMsg))
                return await SuccessResponse(data);
            return await Task.Run(() =>
            {
                return new ApiResponseModel(errMsg, false);
            });        
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authHeaderKey = "CustomAuth";
            if(context.HttpContext.Request.Headers.ContainsKey(authHeaderKey))
            {
                var authHeader = context.HttpContext.Request.Headers[authHeaderKey];
            }

            await base.OnActionExecutionAsync(context, next);
        }

    }
}
