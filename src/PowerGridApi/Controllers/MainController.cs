using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerGridApi.Controllers
{
    [Route("api/[controller]")]
    public class MainController : Controller
    {
        private static EnergoServer _server;

        public static EnergoServer EnergoServer
        {
            get
            {
                if (_server == null)
                {
                    _server = new EnergoServer(new ServerSettings() { SimpleOrGuidPlayerId = true });
                    ServerContext.InitCurrentContext(_server, new Logger());
                }
                return _server;
            }
        }

        [HttpGet("Map/{mapId}")]
        public MapMessageModel GetMap(string mapId)
        {
            var ret = new MapMessageModel();
            if (string.IsNullOrWhiteSpace(mapId))
                mapId = Constants.CONST_DEFAULT_MAP_ID;
            var errMsg = string.Empty;
            var map = EnergoServer.LookupMap(mapId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                ret.Message = errMsg;
            }
            else
            {
                ret.IsSuccess = true;
                ret.Map = map;
            }
            return ret;
        }

        private MessageModel FormatReturn(string errMsg, object data = null)
        {
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new MessageModel(errMsg, false);
            return new MessageModel(data);
        }

        private GameRoomsModel FormatGRReturn(string errMsg, GameRoomViewModel[] rooms = null)
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

        [HttpGet("Login")]
        public MessageModel Login(string username, string userId = null)
        {
            var errMsg = string.Empty;
            userId = EnergoServer.Login(username, out errMsg, userId);
            return FormatReturn(errMsg, userId);
        }

        [HttpGet("CreateRoom")]
        public MessageModel CreateGameRoom(string userId, string name)
        {
            var errMsg = string.Empty;
            var gameRoomId = EnergoServer.CreateGameRoom(userId, name, out errMsg);
            return FormatReturn(errMsg, gameRoomId);
        }

        [HttpGet("RoomList")]
        public GameRoomsModel GetGameRoomList(string userId)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.GetGameRoomList(userId, out errMsg, null);
            var rvm = new List<GameRoomViewModel>();
            foreach (var r in rooms)
                rvm.Add((GameRoomViewModel)r.ToViewModel());
            return new GameRoomsModel()
            {
                GameRooms = rvm.ToArray(),
                Message = errMsg,
                IsSuccess = string.IsNullOrWhiteSpace(errMsg)
            };
        }

        [HttpPost("RoomList1")]
        public GameRoomsModel GetGameRoomList(string userId, RoomsViewModelOptions options = null, RoomLookupSettings lookupSettings = null)
        {
            var errMsg = string.Empty;
            var rooms = EnergoServer.GetGameRoomList(userId, out errMsg, lookupSettings);
            var rvm = new List<GameRoomViewModel>();
            foreach (var r in rooms)
                rvm.Add((GameRoomViewModel)r.ToViewModel(options));
            return new GameRoomsModel()
            {
                GameRooms = rvm.ToArray(),
                Message = errMsg,
                IsSuccess = string.IsNullOrWhiteSpace(errMsg)
            };
        }

        [HttpGet("Join")]
        public MessageModel JoinGameRoom(string userId, string gameRoomId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            var gameRoom = EnergoServer.LookupGameRoom(userId, gameRoomId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            gameRoom.Join(player, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(errMsg, gameRoom.Id);
        }

        [HttpGet("Leave")]
        public MessageModel LeaveGameRoom(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (player.GameRoomRef == null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS);
            player.GameRoomRef.Leave(userId);
            if (player.GameRoomRef != null)
                return FormatReturn(Constants.Instance.CONST_ERR_MSG_YOU_CANT_LEAVE_THIS_GAME_ROOM);
            return FormatReturn(errMsg);
        }

        [HttpGet("Kick")]
        public MessageModel Kick(string userId, string username)
        {
            var errMsg = string.Empty;
            var leader = EnergoServer.LookupPlayer(userId, out errMsg);
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

        [HttpGet("StartGame")]
        public MessageModel StartGame(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (player.GameRoomRef != null)
            {
                player.GameRoomRef.Init(out errMsg);
                player.GameRoomRef.GameBoardRef.Start();
            }
            return FormatReturn(errMsg);
        }

        [HttpGet("GameStatus")]
        public GameBoardModel GetGameStatus(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new GameBoardModel() { IsSuccess = false, Message = errMsg };
            if (player.GameRoomRef == null || player.GameRoomRef.GameBoardRef == null)
                return new GameBoardModel() { IsSuccess = false, Message = "Not in game" };
            return new GameBoardModel()
            {
                IsSuccess = true,
                GameBoard = (GameBoardViewModel)player.GameRoomRef.GameBoardRef.ToViewModel()
            };
        }

        [HttpGet("PlayerInfo")]
        public PlayerModel GetPlayerInfo(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return new PlayerModel() { IsSuccess = false, Message = errMsg };
            return new PlayerModel()
            {
                IsSuccess = true,
                PlayerInfo = (PlayerViewModel)player.ToViewModel()
            };
        }

        [HttpGet("SetReadyMark")]
        public MessageModel SetReadyMarkTo(string userId, bool state)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            var ret = player.GameRoomRef.SetReadyMarkTo(player, state, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(null, ret);
        }


        //IN GAME

        [HttpGet("AllowedActions")]
        public MessageModel GetAllowedActions(string userId)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (!player.IsInGame())
                return FormatReturn("Not in game");
            var lst = player.GameRoomRef.GameBoardRef.GetAllowedActions(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            return FormatReturn(null, lst);
        }

        [HttpGet("DoAction")]
        public MessageModel DoAction(string userId, GameActionEnum action)
        {
            var errMsg = string.Empty;
            var player = EnergoServer.LookupPlayer(userId, out errMsg);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return FormatReturn(errMsg);
            if (!player.IsInGame())
                return FormatReturn("Not in game");
            var gbRef = player.GameRoomRef.GameBoardRef;
            switch (action)
            {
                case GameActionEnum.AuctionPass:
                    if (!gbRef.AuctionPass(userId, out errMsg))
                        return FormatReturn(errMsg);
                    return FormatReturn(null, true);
            }
            return FormatReturn("Incorrect action");
        }
    }
}
