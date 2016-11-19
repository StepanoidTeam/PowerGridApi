using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{

    public class GameRoom : BaseEnergoEntity
    {
        public int MaxPlayers { get; private set; }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public IDictionary<string, PlayerInRoom> Players { get; private set; }

        public User Leader { get; private set; }

        private static Type[] notInGameStages = new[] { typeof(CreateGameStage)/* todo , typeof(FinishGameStage)*/ };

        //todo really, do we need it here?
        public bool IsInGame
        {
            get
            {
                return !notInGameStages.Contains(Stages.CurrentStage.GetType());
            }
        }

        public GameStages Stages { get; private set; }

        //public Round Round { get; private set; }

        public GameRoom(string name, User leader)
        {
            if (leader.GameRoomRef != null)
                throw new Exception(Constants.Instance.ErrorMessage.Is_In_Game_Room);
            if (string.IsNullOrWhiteSpace(name))
                name = GenerateGameRoomName(leader);
            Name = name;

            //be sure game room id is unique
            Id = EnergoServer.GenerateSmallUniqueId();
            //todo setup it according to map settings and also allow to set it in room settings
            MaxPlayers = EnergoServer.MaxPlayersInRoom;

            Players = new Dictionary<string, PlayerInRoom>();
            Players.Add(leader.Id, new PlayerInRoom(leader));
            Leader = leader;

            //todo push mapId here
            //maybe allow to change map (it means and max players too) when room already created?
            var mapId = string.Empty;
            //todo generate context somewhere else
            var gameContext = new GameContext(this, mapId);

            EnergoServer.Current.CreateGameRoom(leader, this);

            Stages = new GameStages(gameContext)
                .AddStage<CreateGameStage>()
                .AddStage<FirstStage>()
                .Start();
        }

        private string GenerateGameRoomName(User leader)
        {
            var name = string.Format("{0}'s Room", leader.Username);
            if (!name.CheckIfNameIsOk())
                name = string.Format("New Room {0}", EnergoServer.Current.GetRoomsQty() + 1);
            return name;
        }

        private void Close(string leaderId)
        {
            if (leaderId == Leader.Id)
            {
                EnergoServer.Current.RemoveGameRoom(Leader, this);
            }
        }

        private void RemoveUser(User user)
        {
            if (user == null)
                return;
            if (Players.ContainsKey(user.Id))
            {
                if (IsInGame)
                    FinishGame(user.Id);
                else
                {
                    Players[user.Id].Player.GameRoomRef = null;
                    Players.Remove(user.Id);
                    if (Players.Count < 1)
                        Close(Leader.Id);
                    else if (user.Id == Leader.Id)
                    {
                        Leader = Players.Values.First().Player;
                    }
                }
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
                //IsInGame = false;
                //todo
                Close(playerId);
            }
        }

        public void Leave(User user)
        {
            RemoveUser(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leaderId"></param>
        /// <param name="user"></param>
        /// <param name="errMsg"></param>
        /// <returns>kicked user id</returns>
        public string Kick(User leader, User user, out string errMsg)
        {
            errMsg = string.Empty;
            if (leader == null || Leader.Id != leader.Id)
            {
                errMsg = Constants.Instance.ErrorMessage.Only_Leader_Can_Kick;
                return null;
            }
            var userId = user.Id;
            RemoveUser(user);
            return userId;
        }

        public bool CanJoin(User player, out string errMsg)
        {
            errMsg = string.Empty;
            if (player.GameRoomRef != null)
                errMsg = Constants.Instance.ErrorMessage.Already_In_The_Game;
            else if (IsInGame)
                errMsg = Constants.Instance.ErrorMessage.Cant_Join_To_Game_In_Proc;
            else if (Players.ContainsKey(player.Id))
                errMsg = Constants.Instance.ErrorMessage.Already_In_This_Game;
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
    }
}