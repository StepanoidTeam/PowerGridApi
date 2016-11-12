﻿using PowerGridEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    /// <summary>
    /// There are allowable only one Energo Server instance in Application, so even if you create it via constructor - it will override EnergoServer.Current.
    /// Be careful, only last created instance will be available (anothers will be disposed)
    /// </summary>
	public partial class EnergoServer
	{
        private static EnergoServer _current;

        /// <summary>
        /// You can use this singleton instance or create new one (just for case you want to init settings right while creating), as you wish
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

        /// <summary>
        /// Same
        /// </summary>
        /// <param name="settings"></param>
		public EnergoServer(ServerSettings settings = null)
		{
            _current = this;

            Settings = settings == null ? new ServerSettings() : settings;
			Users = new Dictionary<string, User>();
			Maps = new Dictionary<string, Map>();
			GameRooms = new Dictionary<string, GameRoom>();
            var mapCreator = new DefaultMapCreator();
            var defaultMapInit = mapCreator.Map;
		}

        public T RouteAction<T>(UserAction action) where T: ActionResponse
        {
            return (T)action.User.GameRoomRef.CurrentStage.RouteAction<ActionResponse>(action);
        }
	}
}
