using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
	public partial class EnergoServer
	{
        private static EnergoServer _current;

        /// <summary>
        /// You can use this singleton instance or create new one (and store it somewhere else), as you wish
        /// </summary>
        public static EnergoServer Current
        {
            get
            {
                if (_current == null)
                    _current = new EnergoServer();
                return _current;
            }
        }

		private IDictionary<string, User> Users { get; set; }
		private IDictionary<string, Map> Maps { get; set; }
		private IDictionary<string, GameRoom> GameRooms { get; set; }
		public ServerSettings Settings { get; set; }

		public EnergoServer(ServerSettings settings = null)
		{
            Settings = settings == null ? new ServerSettings() : settings;
			Users = new Dictionary<string, User>();
			Maps = new Dictionary<string, Map>();
			GameRooms = new Dictionary<string, GameRoom>();
            var mapCreator = new DefaultMapCreator();
			RegisterMap(mapCreator.Map);
		}
        
	}
}
