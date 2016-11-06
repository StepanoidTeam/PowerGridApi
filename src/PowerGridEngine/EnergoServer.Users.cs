using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public partial class EnergoServer
    {
        public User Login(string username, out string errMsg, string userId = null)
        {
            errMsg = string.Empty;

            //check if username valid
            if (string.IsNullOrWhiteSpace(username) || username.Trim().Length < Constants.Instance.CONST_USERNAME_MIN_SIZE)
            {
                errMsg = Constants.Instance.ErrorMessage.Too_Short_Username;
                return null;
            }

            string id = null;
            
            //set userId to pre-setted
            if (!string.IsNullOrWhiteSpace(userId))
            {
                id = userId;
                //check if such user id is exists in system
                if (Users.ContainsKey(id))
                {
                    errMsg = Constants.Instance.ErrorMessage.Similar_User_Detected;
                    return null;
                }
            }
            else //or generate new
            {
                do
                {
                    //if need to generate id just as normalized username and if username is ok for this (no such ids in system yet):
                    if (Settings.SimpleOrGuidPlayerId && id == null)
                        id = username.NormalizeId();
                    else //try to generate new unique Id
                        id = StringExtensions.GenerateId();
                }
                while (Users.ContainsKey(id));
            }
                       
            var p = new User(username, id);
            Users.Add(p.Id, p);
            return p;
        }

        public bool Logout(User user)
        {
            if (!Users.ContainsKey(user.Id))
                return false;
            return Users.Remove(user.Id);
        }

        /// <summary>
        /// If player is authorized in system returns userId
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns>User Id or null if not authorized</returns>
        public string CheckUserAuthToken(string authToken)
        {
            var id = authToken.NormalizeId();
            if (Users.Values.Any(m => m.AuthToken == id))
                return id;
            return null;
        }

        /// <summary>
        /// Check if player is authorized in system
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckIfAuthorized(string authToken)
        {
            return CheckUserAuthToken(authToken) != null;
        }

        public User FindUserByAuthToken(string authToken, out string errMsg)
        {
            errMsg = string.Empty;
            var _authToken = CheckUserAuthToken(authToken);
            if (_authToken == null)
            {
                errMsg = Constants.Instance.ErrorMessage.YouAre_Unauthorized;
                return null;
            }
            return Users.Values.FirstOrDefault(m => _authToken == m.AuthToken);
        }

        public User LookupUserByName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;
            var keyword = username.RemoveExtraSpaces().ToLowerInvariant();
            return Users.Values.FirstOrDefault(m => m.Username.ToLowerInvariant() == keyword);
        }

        public string LookupUserId(string username)
        {
            var p = LookupUserByName(username);
            if (p != null)
                return p.Id;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itsYou">true if lookup for current user, if trying to find another user - set to false. It depends what message you will get if can't find</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public User LookupUserByAuthToken(string authToken, out string errMsg)
        {
            errMsg = string.Empty;
            var user = FindUserByAuthToken(authToken, out errMsg);
            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errMsg"></param>
        /// <param name="itsYou">true if lookup for current user, if trying to find another user - set to false. It depends what message you will get if can't find</param>
        /// <returns></returns>
        public User LookupUser(string userId, out string errMsg, bool itsYou = true)
        {
            errMsg = string.Empty;
            var id = userId.NormalizeId();
            if (!Users.ContainsKey(id))
            {
                if (!itsYou)
                    errMsg = Constants.Instance.ErrorMessage.Cant_Find_Such_User;
                return null;
            }
            return Users[id];
        }
    }
}
