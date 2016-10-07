using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public class RoomLookupSettings
    {
        
        public string Id { get; set; }

        
        public bool CurrentPlayerInside { get; set; }

        public RoomLookupSettings()
        {
        }
    }
}
