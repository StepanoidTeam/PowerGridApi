using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public class GameBoardViewModel : BaseEnergoViewModel
    {
        
        public GameStatusEnum Status { get; set; }

        
        public string[] PlayersTurnOrder { get; set; }

		
		public int? CurrentTurnPlayer { get; set; }

        
        public IDictionary<string,string> PlayersInfo { get; set; }
    }
}
