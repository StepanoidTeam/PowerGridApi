using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public class PlayerViewModel : BaseEnergoModel
    {
        
        public string UserId { get; set; }

        
        public string Username { get; set; }

        
        public string GameRoomId { get; set; }

        
        public bool? ReadyMark { get; set; }

    }
}
