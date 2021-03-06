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
        public static readonly string AdminId = "fa12b1e1";
        public static readonly string AdminAuthToken = "65af5923-a9c7-4207-a446-3970ddfc9b5b";

        private static object _lock = new object();

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
            MaxPlayersInRoom = 6;
            _uniqueIds = new Dictionary<string, object>();
            _current = this;

            Settings = settings == null ? new ServerSettings() : settings;
            Users = new Dictionary<string, User>();
            Maps = new Dictionary<string, Map>();
            GameRooms = new Dictionary<string, GameRoom>();

            var admin = new User("Admin", AllocateSmallUniqueId(AdminId));
            Users.Add(admin.Id, admin);

            var mapCreator = new DefaultMapCreator();
            var defaultMapInit = mapCreator.Map;
        }

        public static void Init()
        {
            _current = new EnergoServer();
        }

        public T RouteAction<T>(UserAction<T> action) where T : ActionResponse
        {
            //todo do we need to check on GameRoomRef for ANY action?
            if (action == null || action.User == null || action.User.GameRoomRef == null)
                return action.CreateErrorResponse("Unexpected error");
            return action.User.GameRoomRef.Stages.Current.RouteAction(action);
        }

        public static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        private static Dictionary<string, object> _uniqueIds;

        public static string GenerateSmallUniqueId()
        {
            lock (_lock)
            {
                var newId = string.Empty;
                do
                {
                    newId = GenerateId().Substring(0, 8);
                } while (_uniqueIds.ContainsKey(newId));
                _uniqueIds.Add(newId, null);
                return newId;
            }
        }

        public static string AllocateSmallUniqueId(string id)
        {
            lock (_lock)
            {
                _uniqueIds.Add(id, null);
                return id;
            }
        }
    }
}
