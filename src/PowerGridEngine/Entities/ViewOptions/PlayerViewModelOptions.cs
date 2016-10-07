using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public class PlayerViewModelOptions : IViewModelOptions
    {
        
        public bool Id { get; set; }

        
        public bool Name { get; set; }

        
        public bool GameRoomId { get; set; }

        
        public bool ReadyMark { get; set; }

        public PlayerViewModelOptions(bool defaultValue = false)
        {
            Id = defaultValue;
            Name = defaultValue;
            GameRoomId = defaultValue;
            ReadyMark = defaultValue;
        }
    }
}
