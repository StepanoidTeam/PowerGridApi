using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace PowerGridApi.Controllers
{
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        public static decimal version = 0.01m;

        // GET api/values
        [HttpGet]
        public string Version()
        {
            return string.Format("v{0}", version.ToString(CultureInfo.InvariantCulture));
        }
        
    }
}
