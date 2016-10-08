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
    [Route("api/[controller]")]
    public class MapsController : BaseController
    {
        /// <summary>
        /// Get list of registered maps
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public MessageModel GetAll()
        {
            var map = EnergoServer.Current.GetAllMapIds();
            return new MessageModel(map);
        }

        /// <summary>
        /// Get map details
        /// </summary>
        /// <param name="mapId">map id could by found in map list</param>
        /// <returns></returns>
        [HttpGet("{mapId}")]
        public MapMessageModel GetMap(string mapId)
        {
            var ret = new MapMessageModel();
            if (string.IsNullOrWhiteSpace(mapId))
                mapId = Constants.CONST_DEFAULT_MAP_ID;
            var errMsg = string.Empty;
            var map = EnergoServer.Current.LookupMap(mapId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                ret.Message = errMsg;
            }
            else
            {
                ret.IsSuccess = true;
                ret.Map = map;
            }
            return ret;
        }

    

    }
}
