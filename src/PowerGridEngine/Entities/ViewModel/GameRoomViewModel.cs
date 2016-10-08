using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public class GameRoomViewModel : BaseEnergoModel
    {
        
        public string Id { get; set; }

        
        public string Name { get; set; }

        
        public bool? IsInGame { get; set; }

        
        public int? PlayerCount { get; set; }

        
        public IdNameModel[] PlayerHeaders { get; set; }

        
        public PlayerViewModel[] PlayerDetails { get; set; }

    }
}
