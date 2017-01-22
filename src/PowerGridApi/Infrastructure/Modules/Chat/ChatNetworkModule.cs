using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using PowerGridApi.Controllers;
using System.Collections.Concurrent;

namespace PowerGridApi
{
    public enum CheckAccessRule
    {
        None,
        HasInviteOrInWhiteListAndNotInBlackList,
        IsSubscribed
    }

    public class ChatNetworkModule : DuplexNetworktHandler
    {
        private object _lockUCM = new object();
        private object _lockGetUserChannels = new object();
        
        private const string ErrMsg_SuchChannelExists = "Such channel already exists";
        private const string ErrMsg_CantDropChannelType = "Is not allowing to drop such channel type";

        public readonly static string ErrMsg_UserInBlackList = "This user inside black list";
        public readonly static string ErrMsg_UserInNotInWhiteList = "You are not in whitelist in channel with active whitelist";
        public readonly static string ErrMsg_UserDontHaveInvite = "You don't have invite to this channel";
        public readonly static string ErrMsg_AlreadyInThisChannel = "Already in this channel";        
        public readonly static string ErrMsg_NoSuchChannelOrNotAllow = "No such channel or you are not allow to {0}";
        public readonly static string ErrMsg_NoSuchChannel = "No such channel";
        public readonly static string ErrMsg_CantDropChannelWithSubscribers = "Channel couldn't be closed while there are users";
        public readonly static string ErrMsg_UserIsNotSubscribed = "You didn't joined in this channel";

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

            message.SenderId = user.Id;
            message.SenderName = user.Username;
            message.Date = DateTime.UtcNow;

            var channel = LookupChannel(user, message.ChannelId,
                message.ChannelId == null ? ChatChannelType.Global : (ChatChannelType?)null);
            if (channel == null)
                return new ApiResponseModel(ErrMsg_NoSuchChannel, ResponseType.InvalidModel);

            //Private don't have restriction, anybody can send to private
            if (channel.Type != ChatChannelType.Private && !channel.Subscribers.ContainsKey(user.Id))
                return new ApiResponseModel(ErrMsg_UserIsNotSubscribed, ResponseType.NotAllowed);

            channel.AddMessage(user, message);
            return new ApiResponseModel(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="checkAccess">if None - do not check any permission,
        /// Otwerwise depends from type (Global - always has access, Room - if in room, Private - only for self private). For Custom:
        /// if HasInviteOrInWhiteListAndNotInBlackList - check if has invite (or in whiteList) and not in blackList - exception on error,
        /// if IsSubscribed - check if subscribed and return null if not</param>
        /// <returns></returns>
        public ChatChannel LookupChannel(User user, string id = null, ChatChannelType? type = null, CheckAccessRule checkAccess = CheckAccessRule.None)
        {
            var channel = Channels.FirstOrDefault(m => (id == null || m.Id == id) && (type == null || type == m.Type));
            if (channel != null && checkAccess != CheckAccessRule.None)
            {
                if (channel.Type == ChatChannelType.Global)
                    return channel;
                if (channel.Type == ChatChannelType.Private)
                    return user.Id == id ? channel : null;
                if (channel.Type == ChatChannelType.Room)
                    return (user.GameRoomRef == null || user.GameRoomRef.Id != id) ? null : channel;

                if (checkAccess == CheckAccessRule.HasInviteOrInWhiteListAndNotInBlackList)
                    channel.CheckPermissions(user.Id);
                else
                {
                    if (!channel.Subscribers.ContainsKey(user.Id))
                        return null;
                }
            }
            return channel;
        }

        public ChatChannel AddChannel(User user, ChatChannelType type, string id = null, string name = null)
        {
            //string subscriberId = null;
            if (type == ChatChannelType.Custom)
            {
                id = null; //can't setup id for custom, it should be generated by system
            }
            else
            {
                var sameChannel = LookupChannel(user, id, type);
                if (sameChannel != null)
                    throw new ArgumentException(ErrMsg_SuchChannelExists);
            }

            if (type == ChatChannelType.Room)
            {
                var room = ServerContext.Current.Server.TryToLookupRoom(id);
                if (room == null)
                    throw new ArgumentException("No such room");
                //subscriberId = room.Id;
            }
            else if (type == ChatChannelType.Private)
            {
                var priv = ServerContext.Current.Server.TryToLookupUser(id);
                if (priv == null)
                    throw new ArgumentException("No such user");
                //subscriberId = priv.Id;
            }

            var channel = new ChatChannel(type, id, name);

            Channels.Add(channel);
            
            //if (subscriberId != null)
            //    channel.Subscribe(subscriberId);
            //else 
            if (type == ChatChannelType.Custom)
                Join(user, channel);

            return channel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="checkAccess">if null - do nothing, if false - check if has invite, if true - check if subscribed</param>
        public void DropChannel(User user, string id, CheckAccessRule checkAccess = CheckAccessRule.None)
        {
            var channel = LookupChannel(user, id, checkAccess: checkAccess);
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

            if (channel.Type == ChatChannelType.Custom)
            {
                if (channel.Subscribers.Count > 1)
                    throw new Exception(ErrMsg_CantDropChannelWithSubscribers);
            }

            Channels.RemoveItem(channel);
        }

        public ChatChannel Join(User user, string channelId)
        {
            var channel = LookupChannel(user, channelId, ChatChannelType.Custom, CheckAccessRule.HasInviteOrInWhiteListAndNotInBlackList);
            if (channel == null || channel.Subscribers.ContainsKey(user.Id))
                throw new Exception(string.Format(ErrMsg_NoSuchChannelOrNotAllow, "join into channel"));

            return Join(user, channel);
        }

        public ChatChannel Join(User user, ChatChannel channel, bool checkAccess = false)
        {
            channel.Subscribe(user, checkAccess);
            return channel;
        }

        public ChatChannel Leave(User user, string channelId)
        {
            var channel = LookupChannel(user, channelId, ChatChannelType.Custom, CheckAccessRule.IsSubscribed);
            if (channel == null)
                throw new Exception(string.Format(ErrMsg_NoSuchChannelOrNotAllow, "leave from channel"));

            if (!channel.Unsubscribe(user))
                throw new Exception("You are not in this channel");

            return channel;
        }

        public void InviteToChannel(User inviter, string inviteWhoId, string channelId)
        {
            var channel = ServerContext.Current.Chat.LookupChannel(inviter, channelId, ChatChannelType.Custom, CheckAccessRule.IsSubscribed);
            if (channel == null)
                throw new Exception(string.Format(ErrMsg_NoSuchChannelOrNotAllow, "invite into this channel"));

            var user = ServerContext.Current.Server.TryToLookupUser(inviteWhoId);
            if (user == null)
                throw new Exception("No such user");

            if (channel.Subscribers.ContainsKey(user.Id))
                throw new Exception(ErrMsg_AlreadyInThisChannel);

            channel.AddOrUpdateAccess(user, true);
        }

        /// <summary>
        /// Add user to white/black list
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channelId"></param>
        /// <param name="toBlackOrWhiteList"></param>
        public void ChangePermission(User user, string channelId, bool toBlackOrWhiteList)
        {
            var channel = ServerContext.Current.Chat.LookupChannel(user, channelId, ChatChannelType.Custom);
            if (channel == null)
                throw new Exception(string.Format(ErrMsg_NoSuchChannelOrNotAllow, "change permission for selected user"));

            channel.AddOrUpdateAccess(user, toBlackOrWhiteList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="isJoinOrInvite">false if just invite</param>
        public void AddOrUpdateChannelToUser(User user, ChatChannel channel, bool isJoinOrInvite)
        {
            lock (_lockUCM)
            {
                if (!userChannelsMap.ContainsKey(user.Id))
                    userChannelsMap.Add(user.Id, new Dictionary<string, bool>());

                if (userChannelsMap[user.Id].ContainsKey(channel.Id))
                    userChannelsMap[user.Id][channel.Id] = isJoinOrInvite;
                else
                    userChannelsMap[user.Id].Add(channel.Id, isJoinOrInvite);
            }
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
                        var chan = LookupChannel(user, channel.Key);
                        if (chan != null)
                            result.Add(chan, channel.Value);
                    }
                }
            }

            return result;
        }
    }
}
