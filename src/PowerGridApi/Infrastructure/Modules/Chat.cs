using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using PowerGridApi.Controllers;

namespace PowerGridApi
{ 
	public class Chat: DuplexNetworktHandler
    {
        public Chat() 
        {
            WebSocketManager.Current.OnMessage += Chat_OnMessage;
            WebSocketManager.Current.OnClose += Chat_OnClose;
        }

        private void Chat_OnClose(User user)
        {
            //to do?
        }

        public void Chat_OnMessage(User user, DuplexNetworkRequestType type, string json)
        {
            var request = TryToGetSpecificRequest<ChatSendModel>(type, json);
            if (request != null)
                ReceiveMessage(user, request);
        }

        public ApiResponseModel ReceiveMessage(User user, ChatSendModel message)
		{
			if (message.InRoomChannel && !user.IsInRoom())
				return new ApiResponseModel(Constants.Instance.ErrorMessage.Not_In_Room, ResponseType.NotAllowed);

            string receiver = null;
            if (message.InRoomChannel)
                receiver = user.GameRoomRef.Id;
            else if (!string.IsNullOrWhiteSpace(message.To))
                receiver = message.To;

            WebSocketManager.Current.Broadcast(message, receiver);

            return new ApiResponseModel(true);
		}
        
	}
}
