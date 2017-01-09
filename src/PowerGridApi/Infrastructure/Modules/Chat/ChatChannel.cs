
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
	public class ChatChannel: BaseEnergoEntity
    {
        private object _lockSubscr = new object();
        private object _lockAccessLst = new object();

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
        /// Key - user Id (Custom type) or room Id (Room type), value - when subscribed (to control which messages could see). 
        /// Ignored for Global channel type, in this case just sending message for ALL.
        /// For Private ignored too and just used channel id (receiver) and sender to broadcast them messages.
        /// </summary>
        public Dictionary<string, DateTime> Subscribers
        {
            get
            {
                lock (_lockSubscr)
                    return _subscribers;
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
                Name = name;
            else
                Name = Type.ToString();

            _subscribers = new Dictionary<string, DateTime>();
            AccessList = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Return false if already subscribed
        /// </summary>
        /// <param name="id">user or room id</param>
        /// <returns></returns>
        public void Subscribe(string id)
        {
            lock (_lockSubscr)
            {
                if (_subscribers.ContainsKey(id))
                    throw new Exception(ChatNetworkModule.ErrMsg_AlreadyInThisChannel);

                if (IsBlackListActive && BlackList.Contains(id))
                    throw new Exception(ChatNetworkModule.ErrMsg_UserInBlackList);

                if (IsWhiteListActive && !WhiteList.Contains(id))
                    throw new Exception(ChatNetworkModule.ErrMsg_UserInNotInWhiteList);

                _subscribers.Add(id, DateTime.UtcNow);
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
                if (!_subscribers.ContainsKey(user.Id))
                    return false;
                _subscribers.Remove(user.Id);
                return true;
            }
        }

        public void AddOrUpdateAccess(string userId, bool hasAccess)
        {
            if (AccessList.ContainsKey(userId))
                AccessList[userId] = hasAccess;
            else
                AccessList.Add(userId, hasAccess);
        }
    }
}
