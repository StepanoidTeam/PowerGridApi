using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    /// <summary>
    /// Base controller class for any controller in API
    /// </summary>
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
        protected ApiResponseModel FormatSuccessReturn(object data)
        {
            return new ApiResponseModel(data);
        }

        /// <summary>
        /// Use this type of reponse in case you need to check if it's ok (errMsg is empty) and return data,
        /// or errMsg is not empty and need to return it as an Error response
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ApiResponseModel FormatReturn(string errMsg, object data = null)
        {
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new ApiResponseModel(errMsg, false);
            return FormatSuccessReturn(data);
        }

        protected GameRoomsModel FormatGRReturn(string errMsg, GameRoomModel[] rooms = null)
        {
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new GameRoomsModel()
                {
                    Message = errMsg,
                    IsSuccess = false
                };
            return new GameRoomsModel()
            {
                GameRooms = rooms,
                IsSuccess = true
            };
        }
    }
}
