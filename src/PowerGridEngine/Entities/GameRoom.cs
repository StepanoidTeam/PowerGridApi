using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public class GameRoom : BaseEnergoEntity
    {
        public override BaseEnergoModel ToModel(IViewModelOptions options = null)
        {
            var ret = new GameRoomViewModel();
            var opts = new RoomsViewModelOptions(true);
            if (options != null)
                opts = options as RoomsViewModelOptions;
            if (opts.Id)
                ret.Id = Id;
            if (opts.Name)
                ret.Name = Name;
            if (opts.IsInGame)
                ret.IsInGame = IsInGame;
            if (opts.PlayerCount)
            {
                if (Players == null)
                    ret.PlayerCount = 0;
                else
                    ret.PlayerCount = Players.Count();
            }
            if (opts.PlayerHeaders)
            {
                if (Players == null)
                    ret.PlayerHeaders = new IdNameModel[0];
                else
                {
                    var players = Players.Values.ToArray();
                    ret.PlayerHeaders = new IdNameModel[Players.Count()];
                    for (int i = 0; i < ret.PlayerHeaders.Length; i++)
                        ret.PlayerHeaders[i] = new IdNameModel { Id = players[i].Player.Id, Name = players[i].Player.Username };
                }
            }
            if (opts.PlayerDetails)
            {
                if (Players == null)
                    ret.PlayerDetails = new PlayerViewModel[0];
                else
                {
                    var players = Players.Values.ToArray();
                    ret.PlayerDetails = new PlayerViewModel[Players.Count()];
                    for (int i = 0; i < ret.PlayerDetails.Length; i++)
                        ret.PlayerDetails[i] = (PlayerViewModel)players[i].Player.ToModel(opts.PlayerViewOptions);
                }
            }
            return ret;
        }

        //PROPERTIES:

        public string Id { get; private set; }

        public string Name { get; private set; }

        public IDictionary<string, PlayerInRoom> Players { get; private set; }

        public IDictionary<string, bool> PlayerReadyMarks { get; private set; }

        public Player Leader { get; private set; }

        public bool IsInGame { get; private set; }

        public EnergoServer ServerRef { get; set; }

        public GameBoard GameBoardRef { get; private set; }

        public GameRoom(string name, Player leader, EnergoServer server = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = GenerateMatchName(leader);
            Name = name;
            Id = Guid.NewGuid().ToString();
            Players = new Dictionary<string, PlayerInRoom>();
            Players.Add(leader.Id, new PlayerInRoom(leader));
            Leader = leader;
            IsInGame = false;
            if (server != null)
            {
                server.CreateGameRoom(leader, this);
            }
        }

        private string GenerateMatchName(Player leader)
        {
            return string.Format("{0}'s Room", leader.Username);
        }

        public void Close(string leaderId)
        {
            if (leaderId == Leader.Id)
            {
                ServerRef.RemoveGameRoom(Leader, this);
            }
        }

        public void ClearPlayers(string leaderId)
        {
            if (leaderId == Leader.Id)
            {
                foreach (var p in Players)
                    p.Value.Player.GameRoomRef = null;
                Leader = null;
                Players = null;
            }
        }

        public void FinishGame(string playerId)
        {
            if (IsInGame && Players.ContainsKey(playerId)
                && Players[playerId].Player.GameRoomRef != null && Players[playerId].Player.GameRoomRef.Id == this.Id)
            {
                IsInGame = false;
                //todo
                Close(playerId);
            }
        }

        private void RemovePlayer(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return;
            if (Players.ContainsKey(playerId))
            {
                if (IsInGame)
                    FinishGame(playerId);
                else
                {
                    Players[playerId].Player.GameRoomRef = null;
                    Players.Remove(playerId);
                    if (Players.Count < 1)
                        Close(Leader.Id);
                    else if (playerId == Leader.Id)
                    {
                        Leader = Players.Values.First().Player;
                    }
                }
            }
        }

        public void Leave(string playerId)
        {
            RemovePlayer(playerId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leaderId"></param>
        /// <param name="playerUn"></param>
        /// <param name="errMsg"></param>
        /// <returns>kicked user id</returns>
        public string Kick(string leaderId, string playerUn, out string errMsg)
        {
            errMsg = string.Empty;
            if (Leader.Id != leaderId)
            {
                errMsg = Constants.Instance.CONST_ERR_MSG_ONLY_LEADER_CAN_KICK;
                return null;
            }
            var playerId = ServerRef.LookupPlayerId(playerUn);
            if (string.IsNullOrWhiteSpace(playerId) || !this.Players.ContainsKey(playerId))
            {
                errMsg = Constants.Instance.CONST_ERR_MSG_THERE_NO_SUCH_USER;
                return null;
            }
            RemovePlayer(playerId);      
            return playerId;
        }

        public bool CanJoin(Player player, out string errMsg)
        {
            errMsg = string.Empty;
            if (player.GameRoomRef != null)
                errMsg = Constants.Instance.CONST_ERR_MSG_ALREADY_IN_THE_GAME;
            else if (IsInGame)
                errMsg = Constants.Instance.CONST_ERR_MSG_CANT_JOIN_TO_GAME_IN_PROC;
            else if (Players.ContainsKey(player.Id))
                errMsg = Constants.Instance.CONST_ERR_MSG_ALREADY_IN_THIS_GAME;
            return string.IsNullOrWhiteSpace(errMsg);
        }

        /// <summary>
        /// returns error msg if error, otherwise - successfully joined
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public void Join(Player player, out string errMsg)
        {
            errMsg = string.Empty;
            if (CanJoin(player, out errMsg))
            {
                Players.Add(player.Id, new PlayerInRoom(player));
                player.GameRoomRef = this;
            }
        }

        private bool ReturnError(string err, out string errMsg)
        {
            errMsg = err;
            return false;
        }

        public bool ToogleReadyMark(Player player, out string errMsg)
        {
            errMsg = string.Empty;

            if (player == null)
                return ReturnError(Constants.Instance.CONST_ERR_MSG_PLAYER_CANT_BE_NULL, out errMsg);
            if (player.GameRoomRef == null)
                return ReturnError(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS, out errMsg);
            var players = player.GameRoomRef.Players;
            if (!players.ContainsKey(player.Id))
                return ReturnError(Constants.Instance.CONST_ERR_MSG_YOU_ARE_NOT_IN_THIS_GAME, out errMsg);
            players[player.Id].ReadyMark = !players[player.Id].ReadyMark;
            return players[player.Id].ReadyMark;
        }
        
        public bool SetReadyMarkTo(Player player, bool state, out string errMsg)
        {
            errMsg = string.Empty;

            if (player == null)
                return ReturnError(Constants.Instance.CONST_ERR_MSG_PLAYER_CANT_BE_NULL, out errMsg);
            if (player.GameRoomRef == null)
                return ReturnError(Constants.Instance.CONST_ERR_MSG_YOUARE_OUTSIDE_OF_GAME_ROOMS, out errMsg);
            var players = player.GameRoomRef.Players;
            if (!players.ContainsKey(player.Id))
                return ReturnError(Constants.Instance.CONST_ERR_MSG_YOU_ARE_NOT_IN_THIS_GAME, out errMsg);
            players[player.Id].ReadyMark = state;
            return players[player.Id].ReadyMark;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="mapId">default if not set</param>
        public void Init(out string errMsg, string mapId = null)
        {
            errMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(mapId))
                mapId = Constants.CONST_DEFAULT_MAP_ID;
            var gameContext = new GameContext(this, out errMsg, mapId);
            //todo we really need it?
            GameBoardRef = gameContext.GameBoard;
            IsInGame = true;
        }

    }
}
