using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public class ServerSettings
    {
        /// <summary>
        /// true - simple (just toLower.AndRemoveWhitespaces), false - Guid
        /// </summary>
        public bool SimpleOrGuidPlayerId { get; set; }
    }
}
