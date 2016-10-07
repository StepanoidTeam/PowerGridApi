using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PowerGridEngine
{
    public class GameBoardModel : MessageModel
    {
        public GameBoardViewModel GameBoard { get; set; }
    }
}
