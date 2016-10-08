using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    public abstract class BaseController : Controller
    {
        private static decimal _version = 0.01m;

        public static string Version
        {
            get
            {
                return string.Format("v{0}", _version.ToString(CultureInfo.InvariantCulture));
            }
        }

        protected MessageModel FormatReturn(string errMsg, object data = null)
        {
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new MessageModel(errMsg, false);
            return new MessageModel(data);
        }

        protected GameRoomsModel FormatGRReturn(string errMsg, GameRoomViewModel[] rooms = null)
        {
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new GameRoomsModel()
                {
                    Message = errMsg,
                    IsSuccess = false
                };
            return new GameRoomsModel()
            {
                GameRooms = rooms,
                IsSuccess = true
            };
        }
    }
}
