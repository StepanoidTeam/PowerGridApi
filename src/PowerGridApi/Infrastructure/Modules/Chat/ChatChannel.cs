
using PowerGridEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridApi
{
    /// <summary>
    /// Only custom channel can have custom name, for system ones - name is default.
    /// Any non-custom channels could not be closed manually (room channel closed with room closing, private - with user logout).
    /// Custom channels could be closed only if only one man in there.
    /// Todo: Who can invite to custom (now everyone)
    /// </summary>
	public class ChatChannel : BaseEnergoEntity
    {
        private object _lockSubscr = new object();
        private object _lockAccessLst = new object();
        private object _lockMessages = new object();

        public string Id { get; private set; }

        public ChatChannelType Type { get; private set; }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (Type == ChatChannelType.Custom)
                    name = value;
            }
        }

        private Dictionary<string, DateTime> _subscribers { get; set; }

        /// <summary>
        /// Key - user Id, value - when subscribed (to control which messages could see). 
        /// Only for custom it will return right connected datetime, otherwise it will return current time.
        /// For Global channel type - All connected users (it doesn't mean they are really online, but mostly there are user with recently activity).
        /// For Private there will be senders, who sent their messages recently on current user Private.
        /// For Room - active users in room.
        /// </summary>
        public Dictionary<string, DateTime> Subscribers
        {
            get
            {
                var server = ServerContext.Current.Server;
                switch (Type)
                {
                    case ChatChannelType.Custom:
                        lock (_lockSubscr)
                            return _subscribers;
                    case ChatChannelType.Global:
                        return server.GetUsers().ToDictionary(k => k.Id, v => DateTime.UtcNow);
                    case ChatChannelType.Room:
                        var room = server.TryToLookupRoom(Id);
                        if (room == null)
                            return new Dictionary<string, DateTime>();
                        return room.Players.ToDictionary(k => k.Key, v => DateTime.UtcNow);
                    case ChatChannelType.Private:
                        var lst = Messages.Select(m => m.SenderId).Distinct().ToList();
                        if (!lst.Contains(Id))
                            lst.Add(Id);
                        return lst.ToDictionary(k => k, v => DateTime.UtcNow);
                }
                return null;
            }
        }

        public bool IsBlackListActive { get; set; }

        public bool IsWhiteListActive { get; set; }

        private Dictionary<string, bool> _accessList { get; set; }

        /// <summary>
        /// White/Black list (according to value, means if true - it's whitelist) with userIds.
        /// It's active only if appopriate setting is true (IsBlackListActive, IsWhiteListActive).
        /// But if even whilelist is not active - it's used to check to which channels user have invites.
        /// </summary>
        public Dictionary<string, bool> AccessList
        {
            get
            {
                lock (_lockAccessLst)
                    return _accessList;
            }
            private set
            {
                lock (_lockAccessLst)
                    _accessList = value;
            }
        }

        public IEnumerable<string> BlackList
        {
            get
            {
                return AccessList.Where(m => !m.Value).Select(m => m.Key);
            }
        }

        public IEnumerable<string> WhiteList
        {
            get
            {
                return AccessList.Where(m => m.Value).Select(m => m.Key);
            }
        }

        private List<ChatMessage> _messages { get; set; }

        /// <summary>
        /// White/Black list (according to value, means if true - it's whitelist) with userIds.
        /// It's active only if appopriate setting is true (IsBlackListActive, IsWhiteListActive).
        /// But if even whilelist is not active - it's used to check to which channels user have invites.
        /// </summary>
        public List<ChatMessage> Messages
        {
            get
            {
                lock (_lockMessages)
                    return _messages;
            }
            private set
            {
                lock (_lockMessages)
                    _messages = value;
            }
        }

        /// <summary>
        /// Create new channel in system
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id">useful for Private and Room type to setup the same Unique id that have for example Room. It also good becase it will not allow you to create sevearal channels for Room</param>
        /// <param name="name">is allowable for Custom type only, for other it will be ignored and name created by default</param>
        public ChatChannel(ChatChannelType type, string id = null, string name = null)
        {
            if (id == null || type == ChatChannelType.Custom)
                Id = PowerGridEngine.EnergoServer.GenerateSmallUniqueId();
            else
                Id = id;

            Type = type;

            if (Type == ChatChannelType.Custom && !string.IsNullOrWhiteSpace(name))
                this.name = name;
            else
                this.name = Type.ToString();

            _subscribers = new Dictionary<string, DateTime>();
            AccessList = new Dictionary<string, bool>();
            _messages = new List<ChatMessage>();
        }

        public void CheckPermissions(string userId)
        {
            if (IsBlackListActive && BlackList.Contains(userId))
                throw new Exception(ChatNetworkModule.ErrMsg_UserInBlackList);

            if (!WhiteList.Contains(userId))
            {
                if (IsWhiteListActive)
                    throw new Exception(ChatNetworkModule.ErrMsg_UserInNotInWhiteList);
                else
                    throw new Exception(ChatNetworkModule.ErrMsg_UserDontHaveInvite);
            }
        }

        /// <summary>
        /// Return false if already subscribed
        /// </summary>
        /// <param name="userId">user or room id</param>
        /// <param name="checkAccess"></param>
        /// <returns></returns>
        public void Subscribe(User user, bool checkAccess = false)
        {
            lock (_lockSubscr)
            {
                if (_subscribers.ContainsKey(user.Id))
                    throw new Exception(ChatNetworkModule.ErrMsg_AlreadyInThisChannel);

                if (checkAccess)
                    CheckPermissions(user.Id);

                ServerContext.Current.Chat.AddOrUpdateChannelToUser(user, this, true);

                _subscribers.Add(user.Id, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Return false if not subscribed
        /// </summary>
        /// <param name="user"></param>
        public bool Unsubscribe(User user)
        {
            lock (_lockSubscr)
            {
                //it means if user leave, he can anyway back. Maybe need to drop invite?
                ServerContext.Current.Chat.AddOrUpdateChannelToUser(user, this, false);

                return _subscribers.Remove(user.Id);
            }
        }

        public void AddOrUpdateAccess(User user, bool hasAccess)
        {
            if (AccessList.ContainsKey(user.Id))
                AccessList[user.Id] = hasAccess;
            else
                AccessList.Add(user.Id, hasAccess);

            if (hasAccess)
                ServerContext.Current.Chat.AddOrUpdateChannelToUser(user, this, false);
        }

        public void AddMessage(User user, ChatMessage message)
        {
            Messages.Add(message);

            var subscribers = Subscribers.Keys.ToList();
            if (Type == ChatChannelType.Private)
                subscribers = new List<string> { Id };

            var data = new ChatMessageModel(message).GetInfo(new ChatMessageModelViewOptions(true));
            //todo move this static link outside
            WebSocketManager.Current.Broadcast(data, subscribers);
        }
    }
}
