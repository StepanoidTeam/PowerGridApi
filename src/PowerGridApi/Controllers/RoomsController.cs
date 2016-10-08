﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    /// <summary>
    /// Create your room or join to some existing one
    /// </summary>
    [Route("api/[controller]")]
    public class RoomsController : BaseController
    {
        /// <summary>
        /// Rooms list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("")]
        public MessageModel GetGameRoomList(string userId)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            
            return new GameRoomsModel()
            {
                GameRooms = rooms.Select(m => m.ToModel()).ToArray(),
                Message = errMsg,
                IsSuccess = string.IsNullOrWhiteSpace(errMsg)
            };
        }
        
        [HttpPost("Create/{userId}/{name}")]
        public MessageModel CreateGameRoom(string userId, string name)
        {
            var errMsg = string.Empty;
            var gameRoomId = EnergoServer.Current.CreateGameRoom(userId, name, out errMsg);
            return FormatReturn(errMsg, gameRoomId);
        }
       
        [HttpPost("List")]
        public MessageModel GetGameRoomList(string userId, RoomsViewModelOptions options = null, RoomLookupSettings lookupSettings = null)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.Current.GetGameRoomList(userId, out errMsg, lookupSettings);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);

            return new GameRoomsModel()
            {
                GameRooms = rooms.Select(m => m.ToModel(options)).ToArray(),
                Message = errMsg,
                IsSuccess = string.IsNullOrWhiteSpace(errMsg)
            };
        }

        [HttpPost("Join")]
        public MessageModel JoinGameRoom(string userId, string gameRoomId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            var gameRoom = EnergoServer.Current.LookupGameRoom(userId, gameRoomId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            gameRoom.Join(player, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(errMsg, gameRoom.Id);
        }

        [HttpPost("Leave")]
        public MessageModel LeaveGameRoom(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (player.GameRoomRef == null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);
            player.GameRoomRef.Leave(userId);
            if (player.GameRoomRef != null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOU_CANT_LEAVE_THIS_GAME_ROOM);
            return FormatReturn(errMsg);
        }

        [HttpPost("Kick")]
        public MessageModel Kick(string userId, string username)
        {
            var errMsg = string.Empty;
            var leader = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (leader.GameRoomRef == null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);
            var gameRoom = leader.GameRoomRef;
            var playerId = gameRoom.Kick(leader.Id, username, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (gameRoom.Players.ContainsKey(playerId))
                errMsg = Constants.Instance.CONST_ERR_MSG_YOU_CANT_KICK_THIS_USER;

            return FormatReturn(errMsg);
        }

        /// <summary>
        /// Set if player ready to start or not
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state">in case it's not null - set ready mark to specified value, otherwise it will be toogled according to curent state</param>
        /// <returns></returns>
        [HttpPost("ToogleReady")]
        public MessageModel SetReadyMarkTo(string userId, bool? state = null)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.Current.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            bool result = false;
            if (state.HasValue)
                player.GameRoomRef.SetReadyMarkTo(player, state.Value, out errMsg);
            else
                player.GameRoomRef.ToogleReadyMark(player, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(null, result);
        }

    }
}
