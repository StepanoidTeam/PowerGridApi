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
    /// Generic Responses hadling part of Base controller class for any controller in API
    /// </summary>
    public abstract partial class BaseController : Controller
    {
        /// <summary>
        /// If response is for sure successfull
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task<IActionResult> SuccessResponse(object data)
        {
            return await Task.Run(() =>
            {
                return Ok(new ApiResponseModel(data));
            });
        }

        /// <summary>
        /// If response is for sure error
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        protected async Task<IActionResult> ErrorResponse(string errMsg)
        {
            return await Task.Run(() =>
            {
                return BadRequest(new ApiResponseModel(errMsg, false));
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

        /// <summary>
        /// Use this method if you know type of response as generic end-point for any response
        /// </summary>
        /// <param name="type"></param>
        /// <param name="errMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task<IActionResult> GenericResponse(ResponseType type, string errMsg = null, object data = null)
        {
            switch(type)
            {
                case ResponseType.Ok:
                    return await SuccessResponse(data);
                case ResponseType.Error:
                    return await ErrorResponse(errMsg ?? "Unexpected error");
                case ResponseType.Unauthorized:
                    {
                        var result = new JsonResult(new ApiResponseModel(Constants.Instance.ErrorMessage.YouAre_Unauthorized, false));
                        result.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        return result;
                    }
                default:
                    return NotFound();
            }
        }

    }
}
