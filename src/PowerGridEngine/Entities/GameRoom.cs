using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    
    public class GameRoom : BaseEnergoEntity
    {
       
        public string Id { get; private set; }

        public string Name { get; private set; }

        public IDictionary<string, PlayerInRoom> Players { get; private set; }

        public User Leader { get; private set; }

        public bool IsInGame { get; private set; }

        public EnergoServer ServerRef { get; set; }

        public GameBoard GameBoardRef { get; private set; }

        public GameRoom(string name, User leader, EnergoServer server = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = GenerateGameRoomName(leader);
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

        private string GenerateGameRoomName(User leader)
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

        public bool CanJoin(User player, out string errMsg)
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
        public void Join(User player, out string errMsg)
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

        public bool ToogleReadyMark(User player, out string errMsg)
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
        
        public bool SetReadyMarkTo(User player, bool state, out string errMsg)
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
