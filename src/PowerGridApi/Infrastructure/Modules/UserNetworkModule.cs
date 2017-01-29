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
            ServerContext.Current.DuplexNetwork.OnRequestRecieved += OnRequestRecieved;
            ServerContext.Current.DuplexNetwork.OnClose += OnClose;
        }

        private void OnClose(User user)
        {
            if (user != null)
            {
                ServerContext.Current.DuplexNetwork.SetUserAsOffline(user);
                //todo really don't need?
                //Logout(user);
                //Need to setup some timeout after which log out (or let's user be logged even if it's not online?)
            }
        }

        public void OnRequestRecieved(User user, DuplexNetworkRequestType type, string json)
        {
        }

        public ApiResponseModel Logout(User user)
		{
            var result = EnergoServer.Current.Logout(user);

            if (result)
            {
                ServerContext.Current.Chat.DropChannel(user, user.Id, CheckAccessRule.IsSubscribed);

                ServerContext.Current.DuplexNetwork.SetUserAsOffline(user);

                ServerContext.Current.Logger.Log(LogDestination.Console, LogType.Info, "Logout Id = {0}, name = {1}.", user.Id, user.Username);

                var broadcast = new UserModel(user).GetInfo(new UserModelViewOptions()
                {
                    Id = true
                }).AddItem("BroadcastReason", "Logout");

                ServerContext.Current.DuplexNetwork.Broadcast(broadcast);
                
                return new ApiResponseModel(true);
            }

            return new ApiResponseModel("Couldn't logout", ResponseType.UnexpectedError);
		}
        
	}
}
