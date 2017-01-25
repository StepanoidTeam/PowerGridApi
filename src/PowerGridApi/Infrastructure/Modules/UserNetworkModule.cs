using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using PowerGridApi.Controllers;

namespace PowerGridApi
{ 
	public class UserNetworkModule: DuplexNetworktHandler
    {
        public UserNetworkModule() 
        {
            WebSocketManager.Current.OnMessage += OnMessage;
            WebSocketManager.Current.OnClose += OnClose;
        }

        private void OnClose(User user)
        {
            if (user != null)
            {
                WebSocketManager.Current.ForgotUser(user);
                //todo really don't need?
                //Logout(user);
            }
        }

        public void OnMessage(User user, DuplexNetworkRequestType type, string json)
        {
        }

        public ApiResponseModel Logout(User user)
		{
            var result = EnergoServer.Current.Logout(user);

            if (result)
            {
                ServerContext.Current.Chat.DropChannel(user, user.Id, CheckAccessRule.IsSubscribed);

                WebSocketManager.Current.ForgotUser(user);

                ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, "Logout Id = {0}, name = {1}.", user.Id, user.Username);

                var broadcast = new UserModel(user).GetInfo(new UserModelViewOptions()
                {
                    Id = true
                }).AddItem("BroadcastReason", "Logout");

                WebSocketManager.Current.Broadcast(broadcast);
                
                return new ApiResponseModel(true);
            }

            return new ApiResponseModel("Couldn't logout", ResponseType.UnexpectedError);
		}
        
	}
}
