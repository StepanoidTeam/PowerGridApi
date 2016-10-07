using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public class RoomsViewModelOptions : IViewModelOptions
    {
        
        public bool Id { get; set; }

        
        public bool Name { get; set; }

        
        public bool IsInGame { get; set; }

        
        public bool PlayerCount { get; set; }

        /// <summary>
        /// Id + Name
        /// </summary>
        
        public bool PlayerHeaders { get; set; }
       
        
        public bool PlayerDetails { get; set; }

        
        public PlayerViewModelOptions PlayerViewOptions { get; set; }

        public RoomsViewModelOptions(bool defaultValue = false)
        {
            Id = defaultValue;
            Name = defaultValue;
            IsInGame = defaultValue;
            PlayerCount = defaultValue;
            PlayerHeaders = defaultValue;
            PlayerDetails = defaultValue;
            PlayerViewOptions = new PlayerViewModelOptions(defaultValue);
        }
    }
}
