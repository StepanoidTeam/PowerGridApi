﻿using System;
using System.Collections.Generic;
using System.Linq;
using PowerGridEngine;

namespace PowerGridApi
{
	public abstract class DuplexNetworktHandler
	{
		/// <summary>
		/// Determine if request has concrete type and parse it, if type is not expecting - return null
		/// </summary>
		public T TryToGetSpecificRequest<T>(DuplexNetworkRequestType type, string json) where T : IWebSocketRequestModel
		{
			var data = (json ?? "");
			try
			{
				//todo: dynamically get JSON obj type
				//and create typed obj based on that type
				//no JSON IN JSON !
				var x = data.ToObject();


				switch (type)
				{
					case DuplexNetworkRequestType.Chat:

						

						return (T)Convert.ChangeType(data.ToObject<ChatSendModel>(), typeof(T));
					case DuplexNetworkRequestType.Login:
						return (T)Convert.ChangeType(data.ToObject<LoginModel>(), typeof(T));
				}
			}
			catch
			{
				//nothing to do. It means prefix is not corresponds to type T
			}

			return default(T);
		}
	}
}
