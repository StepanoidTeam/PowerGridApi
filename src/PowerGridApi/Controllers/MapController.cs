﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.SwaggerGen.Annotations;

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
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var maps = EnergoServer.Current.GetAllMapIds();
            return await SuccessResponse(maps);
        }

        /// <summary>
        /// Get map by flexible response customization
        /// </summary>
        /// <param name="mapWithOptions">mapId and options to customize response</param>
        /// <returns></returns>
        [AllowAnonymous]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "NotFound")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Ok")]
        [HttpPost("Map")]
        public async Task<IActionResult> GetMapWithOptions([FromBody]MapWithOptions mapWithOptions)
        {
            var mapId = mapWithOptions.MapId;
            var options = mapWithOptions.Options;
            if (string.IsNullOrWhiteSpace(mapId))
                mapId = Constants.CONST_DEFAULT_MAP_ID;
            var errMsg = string.Empty;
            var map = EnergoServer.Current.LookupMap(mapId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return await GenericResponse(ResponseType.NotFound, errMsg);
           
            var mapModel = new MapModel(map);
            var result = await Task.Run(() => { return mapModel.GetInfo(options); });

            return await SuccessResponse(result);
        }
    }
}
