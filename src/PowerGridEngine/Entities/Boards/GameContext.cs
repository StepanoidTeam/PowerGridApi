﻿using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Сложим все доски в одну корзину, чтобы не держать связи между ними друг в друге.
    /// Будем просто держать все в одном контексте.
    /// </summary>
	public class GameContext
	{
        public bool IsInited { get; private set; }
        
        public IDictionary<string, PlayerBoard> PlayerBoards { get; private set; }

        public GameBoard GameBoard { get; private set; }

        public StationBoard StationBoard { get; private set; }

        /// <summary>
        /// todo удалять контексты при необходимости (игра закончена, игра заброшена, разрыв связи...)
        /// </summary>
        private static List<GameContext> Contexts { get; set; }

        public GameContext(GameRoom room, out string errMsg, string mapId = null)
        {
            GameBoard = new GameBoard(this, out errMsg, mapId);
            if (!string.IsNullOrWhiteSpace(errMsg))
                return;

            PlayerBoards = new Dictionary<string, PlayerBoard>();
            foreach (var p in room.Players)
            {
                PlayerBoards.Add(p.Key, new PlayerBoard(this, p.Value.Player));
            }

            var deck = GameRule.CreateDeck();
            StationBoard = new StationBoard(this, deck);

            IsInited = true;

            if (Contexts == null)
                Contexts = new List<GameContext>();
            Contexts.Add(this);
        }
        
        public static GameContext GetContextByPlayer(Player player)
        {
            return Contexts.FirstOrDefault(m => m.PlayerBoards.Any(n => n.Key == player.Id));
        }
	}
}
