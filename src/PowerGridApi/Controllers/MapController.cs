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
    /// Maps related actions
    /// </summary>
    [Route("api/[controller]")]
    public class MapsController : BaseController
    {
        /// <summary>
        /// Get list of registered maps
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponseModel GetAll()
        {
            var maps = EnergoServer.Current.GetAllMapIds();
            return FormatSuccessReturn(maps);
        }

        /// <summary>
        /// Get map details
        /// </summary>
        /// <param name="mapId">map id could by found in map list</param>
        /// <returns></returns>
        [HttpGet("{mapId}")]
        public ApiResponseModel GetMap(string mapId)
        {
            if (string.IsNullOrWhiteSpace(mapId))
                mapId = Constants.CONST_DEFAULT_MAP_ID;
            var errMsg = string.Empty;
            var map = EnergoServer.Current.LookupMap(mapId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            var mapModel = new MapModel(map);
            return FormatSuccessReturn(mapModel.GetInfo());
        }
  
        //todo something wrong with this method or swagger, it brokes swagger UI loading

        //[HttpPost("WithOptions")]
        //public ApiResponseModel GetMapWithOptions(string mapId, MapModelViewOptions options)
        //{
        //    if (string.IsNullOrWhiteSpace(mapId))
        //        mapId = Constants.CONST_DEFAULT_MAP_ID;
        //    var errMsg = string.Empty;
        //    var map = EnergoServer.Current.LookupMap(mapId, out errMsg);
        //    if (!string.IsNullOrWhiteSpace(errMsg))
        //        return FormatReturn(errMsg);
        //    var mapModel = new MapModel(map);
        //    return FormatSuccessReturn(mapModel.GetInfo());
        //}
    }
}
