using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public class PlayerInRoom
    {
        public Player Player { get; set; }
        public bool ReadyMark { get; set; }

        public PlayerInRoom(Player player)
        {
            Player = player;
        }
    }
}
