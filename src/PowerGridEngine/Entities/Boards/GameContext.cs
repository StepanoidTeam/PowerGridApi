using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Сложим все доски в одну корзину, чтобы не держать связи между ними друг в друге.
    /// Будем просто держать все в одном контексте.
    /// </summary>
	public class GameContext
	{
        public GameRoom Room { get; private set; }

        public IEnumerable<User> Players
        {
            get
            {
                return Room.Players.Values.Select(m => m.Player);
            }
        }

        private IDictionary<string, PlayerBoard> _playerBoards;

        public IDictionary<string, PlayerBoard> PlayerBoards
        {
            get
            {
                var emptyPlayers = Players.Where(m => !_playerBoards.ContainsKey(m.Id));
                foreach (var emptyPlayer in emptyPlayers)
                    _playerBoards.Add(emptyPlayer.Id, new PlayerBoard(this, emptyPlayer));
                return _playerBoards;
            }
        }

        public GameBoard GameBoard { get; private set; }

        public StationBoard StationBoard { get; private set; }

        /// <summary>
        /// todo удалять контексты при необходимости (игра закончена, игра заброшена, разрыв связи?...)
        /// </summary>
        private static List<GameContext> Contexts { get; set; }

        public GameContext(GameRoom room, string mapId = null)
        {
            Room = room;
            GameBoard = new GameBoard(this, mapId);

            _playerBoards = new Dictionary<string, PlayerBoard>();
            foreach (var p in room.Players)
            {
                _playerBoards.Add(p.Key, new PlayerBoard(this, p.Value.Player));
            }

            var deck = GameRule.CreateDeck();
            StationBoard = new StationBoard(this, deck);

            if (Contexts == null)
                Contexts = new List<GameContext>();
            Contexts.Add(this);
        }

        public static GameContext GetContextByPlayer(User player)
        {
            return Contexts.FirstOrDefault(m => m.PlayerBoards.Any(n => n.Key == player.Id));
        }
	}
}
