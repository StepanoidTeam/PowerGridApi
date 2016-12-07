﻿using PowerGridEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridApi
{
	public class ServerContext
	{
        public Chat Chat { get; private set; }

		public EnergoServer Server { get; private set; }

		public ILogger Logger
		{
			get;
			private set;
		}


		public static ServerContext Current
		{
			get;
			private set;
		}

		public static void InitCurrentContext(EnergoServer server, ILogger logger)
		{
			Current = new ServerContext(server, logger);
		}

		public ServerContext(EnergoServer server, ILogger logger)
		{
			Server = server;
			Logger = logger;
            Chat = new Chat();
		}
	}
}