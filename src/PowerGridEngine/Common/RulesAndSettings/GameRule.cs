using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public static class GameRule
    {
        public static bool PaymentTransaction(Player player, int value)
        {
            var context = GameContext.GetContextByPlayer(player);
            if (context == null)
                return false;
            return context.PlayerBoards[player.Id].Payment(value);
        }

        public static void ChangeTurnOrder(GameContext context)
        {
            var playersCount = context.PlayerBoards.Count();
            context.GameBoard.PlayersTurnOrder = new string[playersCount];
            var random = new Random();
            foreach (var p in context.PlayerBoards)
            {
                var flag = true;
                do
                {
                    var order = random.Next(1, playersCount * 10) / 10;
                    if (order < playersCount && string.IsNullOrWhiteSpace(context.GameBoard.PlayersTurnOrder[order]))
                    {
                        context.GameBoard.PlayersTurnOrder[order] = p.Key;
                        flag = false;
                    }
                }
                while (flag);
            }
            context.GameBoard.CurrentTurnPlayer = 0;
        }

        public static List<StationCard> CreateDeck()
        {
            var lst = new List<StationCard>();
            lst.Add(new StationCard(StationType.None, 0, 0, 0));
            lst.Add(new StationCard(StationType.Oil, 3, 2, 1));
            lst.Add(new StationCard(StationType.Coal, 4, 2, 1));
            lst.Add(new StationCard(StationType.Coal | StationType.Oil, 5, 2, 1));
            lst.Add(new StationCard(StationType.Trash, 6, 1, 1));
            lst.Add(new StationCard(StationType.Oil, 7, 3, 2));
            lst.Add(new StationCard(StationType.Coal, 8, 3, 2));
            lst.Add(new StationCard(StationType.Oil, 9, 1, 1));
            lst.Add(new StationCard(StationType.Coal, 10, 2, 2));
            lst.Add(new StationCard(StationType.Atomic, 11, 1, 2));
            lst.Add(new StationCard(StationType.Coal | StationType.Oil, 12, 2, 2));
            lst.Add(new StationCard(StationType.Nature, 13, 0, 1));
            lst.Add(new StationCard(StationType.Trash, 14, 2, 2));
            lst.Add(new StationCard(StationType.Coal, 15, 2, 3));
            lst.Add(new StationCard(StationType.Oil, 16, 2, 3));
            lst.Add(new StationCard(StationType.Atomic, 17, 1, 2));
            lst.Add(new StationCard(StationType.Nature, 18, 0, 2));
            lst.Add(new StationCard(StationType.Trash, 19, 2, 3));
            lst.Add(new StationCard(StationType.Coal, 20, 3, 5));
            lst.Add(new StationCard(StationType.Coal | StationType.Oil, 21, 2, 4));
            lst.Add(new StationCard(StationType.Nature, 22, 0, 2));
            return lst;
        }
    }
}
