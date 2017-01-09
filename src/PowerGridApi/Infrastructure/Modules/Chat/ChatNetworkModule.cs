﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using PowerGridApi.Controllers;
using System.Collections.Concurrent;

namespace PowerGridApi
{
    public class ChatNetworkModule : DuplexNetworktHandler
    {
        private object _lockUCM = new object();
        private object _lockGetUserChannels = new object();
        
        private const string ErrMsg_SuchChannelExists = "Such channel already exists";
        private const string ErrMsg_CantDropChannelType = "Is not allowing to drop such channel type";

        public readonly static string ErrMsg_UserInBlackList = "This user inside black list";
        public readonly static string ErrMsg_UserInNotInWhiteList = "Could not join to channel with active whitelist, you are not in whitelist";
        public readonly static string ErrMsg_AlreadyInThisChannel = "Already in this channel";        
        public readonly static string ErrMsg_NoSuchChannelOrNotAllow = "No such channel or you are not allow to {0}";
        public readonly static string ErrMsg_NoSuchChannel = "No such channel";
        public readonly static string ErrMsg_CantDropChannelWithSubscribers = "Channel couldn't be closed while there are users";

        public ConcurrentBag<ChatChannel> Channels { get; set; }

        /// <summary>
        /// List of channels connected with user (active or just invited to). It includes only Custom type channels.
        /// Key - userId, Value - (key - channelId, value - if is joined, false - just invited but not accepted).
        /// </summary>
        private Dictionary<string, Dictionary<string, bool>> userChannelsMap;
        private Dictionary<string, Dictionary<string, bool>> UserChannelsMap
        {
            get
            {
                lock (_lockUCM)
                {
                    return userChannelsMap;
                }
            }
            set
            {
                lock (_lockUCM)
                {
                    userChannelsMap = value;
                }
            }
        }

        public ChatNetworkModule()
        {
            WebSocketManager.Current.OnMessage += Chat_OnMessage;
            WebSocketManager.Current.OnClose += Chat_OnClose;

            Channels = new ConcurrentBag<ChatChannel>();
            AddChannel(null, ChatChannelType.Global);
            UserChannelsMap = new Dictionary<string, Dictionary<string, bool>>();
        }

        private void Chat_OnClose(User user)
        {
            //to do?
        }

        public void Chat_OnMessage(User user, DuplexNetworkRequestType type, string json)
        {
            ReceiveMessage(user, TryToGetSpecificRequest<ChatSendModel>(type, json));
        }

        public ApiResponseModel ReceiveMessage(User user, ChatSendModel message)
        {
            if (message == null)
                return new ApiResponseModel(false);

            if (message.InRoomChannel && !user.IsInRoom())
                return new ApiResponseModel(Constants.Instance.ErrorMessage.Not_In_Room, ResponseType.NotAllowed);

            message.SenderId = user.Id;
            message.SenderName = user.Username;
            message.Date = DateTime.UtcNow;

            string receiver = null;
            if (message.InRoomChannel)
                receiver = user.GameRoomRef.Id;
            else if (!string.IsNullOrWhiteSpace(message.To))
                receiver = message.To;

            WebSocketManager.Current.Broadcast(message, receiver);

            return new ApiResponseModel(true);
        }

        public ChatChannel LookupChannel(string id = null, ChatChannelType? type = null)
        {
            return Channels.FirstOrDefault(m => (id == null || m.Id == id) && (type == null || type == m.Type));
        }

        public ChatChannel AddChannel(User user, ChatChannelType type, string id = null, string name = null)
        {
            string subscriberId = null;
            if (type == ChatChannelType.Custom)
            {
                id = null; //can't setup id for custom, it should be generated by system
            }
            else
            {
                var sameChannel = LookupChannel(id, type);
                if (sameChannel != null)
                    throw new ArgumentException(ErrMsg_SuchChannelExists);
            }

            if (type == ChatChannelType.Room)
            {
                var room = ServerContext.Current.Server.TryToLookupRoom(id);
                if (room == null)
                    throw new ArgumentException("No such room");
                subscriberId = room.Id;
            }
            else if (type == ChatChannelType.Private)
            {
                var priv = ServerContext.Current.Server.TryToLookupUser(id);
                if (priv == null)
                    throw new ArgumentException("No such user");
                subscriberId = priv.Id;
            }

            var channel = new ChatChannel(type, id, name);

            if (subscriberId != null)
                channel.Subscribe(subscriberId);
            else if (type == ChatChannelType.Custom)
                Join(user, channel);

            Channels.Add(channel);

            return channel;
        }

        public void DropChannel(User user, string id)
        {
            var channel = LookupChannel(id);
            if (channel != null)
                DropChannel(user, channel);
        }

        /// <summary>
        /// Do not allow to drop channels with peoples. Global, Private is not closing. If it's Room - only Leader could drop (no matter if there are users).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        public void DropChannel(User user, ChatChannel channel)
        {
            if (channel.Type == ChatChannelType.Global)
                throw new Exception(ErrMsg_CantDropChannelType);

            if (channel.Type == ChatChannelType.Private && user.Id != channel.Id)
                throw new Exception(ErrMsg_CantDropChannelType);

            if (channel.Type == ChatChannelType.Room)
            {
                var room = ServerContext.Current.Server.TryToLookupRoom(channel.Id);
                if (room == null || user == null || room.Leader.Id != user.Id)
                    throw new Exception(ErrMsg_NoSuchChannelOrNotAllow);
            }
            else if (channel.Type == ChatChannelType.Custom)
            {
                if (channel.Subscribers.Count > 1)
                    throw new Exception(ErrMsg_CantDropChannelWithSubscribers);
            }

            Channels.RemoveItem(channel);
        }

        public ChatChannel Join(User user, string channelId)
        {
            var channel = LookupChannel(channelId, ChatChannelType.Custom);
            if (channel == null)
                throw new Exception(string.Format(ErrMsg_NoSuchChannelOrNotAllow, "join into channel"));

            Join(user, channelId);

            return channel;
        }

        public ChatChannel Join(User user, ChatChannel channel)
        {
            channel.Subscribe(user.Id);
            AddOrUpdateChannelToUser(user, channel, true);

            return channel;
        }


        public ChatChannel Leave(User user, string channelId)
        {
            var channel = LookupChannel(channelId, ChatChannelType.Custom);
            if (channel == null)
                throw new Exception(string.Format(ErrMsg_NoSuchChannelOrNotAllow, "leave from channel"));

            if (!channel.Unsubscribe(user))
                throw new Exception("You are not in this channel");

            //it means if user leave, he can anyway back. Maybe need to drop invite?
            AddOrUpdateChannelToUser(user, channel, false);

            return channel;
        }

        public void InviteToChannel(User inviter, string inviteWhoId, string channelId)
        {
            var channel = ServerContext.Current.Chat.LookupChannel(channelId, ChatChannelType.Custom);
            if (channel == null || !channel.Subscribers.ContainsKey(inviter.Id))
                throw new Exception(string.Format(ErrMsg_NoSuchChannelOrNotAllow, "invite into this channel"));

            var user = ServerContext.Current.Server.TryToLookupUser(inviteWhoId);
            if (user == null)
                throw new Exception("No such user");

            if (channel.Subscribers.ContainsKey(user.Id))
                throw new Exception(ErrMsg_AlreadyInThisChannel);

            channel.AddOrUpdateAccess(inviteWhoId, true);

            AddOrUpdateChannelToUser(user, channel, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="isJoinOrInvite">false if just invite</param>
        public void AddOrUpdateChannelToUser(User user, ChatChannel channel, bool isJoinOrInvite)
        {
            if (!UserChannelsMap.ContainsKey(user.Id))
                UserChannelsMap.Add(user.Id, new Dictionary<string, bool>());

            if (UserChannelsMap[user.Id].ContainsKey(channel.Id))
                UserChannelsMap[user.Id][channel.Id] = isJoinOrInvite;
            else
                UserChannelsMap[user.Id].Add(channel.Id, isJoinOrInvite);
        }

        public Dictionary<ChatChannel, bool> GetUserChannels(User user, bool withSystems = true)
        {
            var result = new Dictionary<ChatChannel, bool>();
            if (withSystems)
            {
                result = ServerContext.Current.Chat.Channels.Where(m => m.Type == ChatChannelType.Global ||
                    (m.Type == ChatChannelType.Private && m.Id == user.Id))
                    .ToDictionary(m => m, v => true);
                ChatChannel roomChannel = null;
                if (user.GameRoomRef != null && (roomChannel = ServerContext.Current.Chat.Channels.FirstOrDefault(m =>
                   m.Id == user.GameRoomRef.Id && m.Type == ChatChannelType.Room)) != null)
                    result.Add(roomChannel, true);
            }

            if (UserChannelsMap.ContainsKey(user.Id))
            {
                lock (_lockGetUserChannels)
                {
                    var map = UserChannelsMap[user.Id];
                    foreach (var channel in map)
                    {
                        var chan = LookupChannel(channel.Key);
                        if (chan != null)
                            result.Add(chan, channel.Value);
                    }
                }
            }

            return result;
        }
    }
}
