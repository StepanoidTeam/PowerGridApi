using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Доска с колодой энергостанций и аукционом
    /// </summary>
    public class StationBoard
    {
        private GameContext context { get; set; }

        /// <summary>
        /// Определяет сколько первых карт отсортированых по возврастанию цены доступны для выбора
        /// </summary>
        private int AuctionHowMuchCanBuy = 4;

        private int AuctionQty { get { return AuctionHowMuchCanBuy * 2; } }

        /// <summary>
        /// Карты отсортированые по возрастанию цены
        /// </summary>
        public SortedDictionary<int, StationCard> Auction { get; set; }

        public Stack<StationCard> Deck { get; private set; }

        public StationBoard(GameContext context, List<StationCard> deck)
        {
            var specificCards = deck.Where(m => m.IsSpecific);
            if (specificCards.Count() != 2)
                throw new ArgumentException("Stations deck must contain 3 Stage card and Natural energy card with cost 13");

            this.context = context;

            var others = deck.Where(m => !m.IsSpecific).OrderBy(m => m.Cost);
            var first8 = others.Take(AuctionQty);

            var rnd = new Random();
            var cards = new[] { specificCards.FirstOrDefault(m => m.Is3StageCard) }     //3 Stage card
                .Concat(others.Skip(AuctionQty).OrderBy(x => rnd.Next()))               //randomized deck of generic cards
                .Concat(new[] { specificCards.FirstOrDefault(m => !m.Is3StageCard) })   //nature energy with 13 cost card
                .Concat(first8);                                                        //first 8 low cost station cards

            Deck = new Stack<StationCard>();

            foreach (var card in cards)
                Deck.Push(card);

            Auction = new SortedDictionary<int, StationCard>();

            FillAuction();
        }

        public void FillAuction()
        {
            for (int i = Auction.Count; i < AuctionQty; i++)
                Auction.Add(Deck.Peek().Cost, Deck.Pop());
        }
        
        public StationCard Buy(int index)
        {
            if (index >= AuctionHowMuchCanBuy || index < 0 || index >= Auction.Count())
                return null;
                //throw new ArgumentException("Incorrect index");

            var card = Auction.ToArray()[index].Value;
            Auction.Remove(card.Cost);
            FillAuction();

            return card;
        }

         
    }
}
