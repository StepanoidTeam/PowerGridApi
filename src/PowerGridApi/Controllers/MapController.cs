using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    /// <summary>
    /// Maps related actions
    /// </summary>
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class MapsController : BaseController
    {
        /// <summary>
        /// Get list of registered maps
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponseModel> GetAll()
        {
            var maps = EnergoServer.Current.GetAllMapIds();
            return await SuccessResponse(maps);
        }

        /// <summary>
        /// Get map details
        /// </summary>
        /// <param name="mapId">map id could by found in map list</param>
        /// <returns></returns>
        [HttpGet("{mapId}")]
        public async Task<ApiResponseModel> GetMap(string mapId)
        {
            if (string.IsNullOrWhiteSpace(mapId))
                mapId = Constants.CONST_DEFAULT_MAP_ID;
            var errMsg = string.Empty;
            var map = EnergoServer.Current.LookupMap(mapId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);

            var mapModel = new MapModel(map);
            var result = await Task.Run(() => { return mapModel.GetInfo(); });

            return await SuccessResponse(result);
        }

        /// <summary>
        /// Get map by flexible response customization
        /// </summary>
        /// <param name="mapWithOptions">mapId and options to customize response</param>
        /// <returns></returns>
        [HttpPost("WithOptions")]
        public async Task<ApiResponseModel> GetMapWithOptions([FromBody]MapWithOptions mapWithOptions)
        {
            var mapId = mapWithOptions.MapId;
            var options = mapWithOptions.Options;
            if (string.IsNullOrWhiteSpace(mapId))
                mapId = Constants.CONST_DEFAULT_MAP_ID;
            var errMsg = string.Empty;
            var map = EnergoServer.Current.LookupMap(mapId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(errMsg);

            var mapModel = new MapModel(map);
            var result = await Task.Run(() => { return mapModel.GetInfo(options); });

            return await SuccessResponse(result);
        }
    }
}
