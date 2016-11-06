using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{	
	public class GameBoard : BaseEnergoEntity
	{
        private GameContext context { get; set; }

		public Map Map { get; private set; }
        
		public GameStatusEnum Status { get; set; }

		public string[] PlayersTurnOrder { get; set; }

		public int CurrentTurnPlayer { get; set; }
        
		public GameBoard(GameContext context, out string errMsg, string mapId = Constants.CONST_DEFAULT_MAP_ID)
		{
            this.context = context;

			errMsg = string.Empty;
			
			Map = ServerContext.Current.Server.LookupMap(mapId, out errMsg);
            
			if (Map != null)
			{
                Status = GameStatusEnum.Auction;
			}
		}
		
        public void Start()
        {
            //todo move this out
            foreach (var p in context.PlayerBoards)
                GameRule.PaymentTransaction(p.Value.PlayerRef, 50);
            GameRule.ChangeTurnOrder(context);
        }

        public bool CheckInStatus(GameStatusEnum status, out string errMsg)
		{
			errMsg = string.Empty;
			if (this.Status != status)
				errMsg = string.Format(Constants.Instance.ErrorMessage.Already_In_The_Game, status.ToString());
			return this.Status == status;
		}

		public bool CheckInUserTurn(string userId, out string errMsg)
		{
			errMsg = string.Empty;
			if (this.PlayersTurnOrder[this.CurrentTurnPlayer] == userId)
				return true;
			var p = context.PlayerBoards.FirstOrDefault(m => m.Key == userId);
			errMsg = string.Format(Constants.Instance.ErrorMessage.Is_Not_Yout_Turn, p.Value.PlayerRef.Username);
			return false;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errMsg">Only error could be if is not user turn now</param>
        /// <returns></returns>
		public IEnumerable<GameActionEnum> GetAllowedActions(string userId, out string errMsg)
        {
            //todo: think it's bad idea to return such error for case Delete Station card in
            //case you bought more cards and need to make decision which one to remove
            //while Action in progress yet and other users are buying stations yet.
			errMsg = string.Empty;
			if (!CheckInUserTurn(userId, out errMsg))
				return null;
			switch(this.Status)
			{
				case GameStatusEnum.Auction:
					return new [] { GameActionEnum.AuctionSelectCard };
			}
			return new GameActionEnum[0];
		}

		public void ChangePlayerTurn()
		{
			CurrentTurnPlayer++;
			if(CurrentTurnPlayer>=PlayersTurnOrder.Length)
			{
				//todo next stage
			}
		}

		public bool AuctionPass(string userId, out string errMsg)
		{
			errMsg = string.Empty;
			if (!CheckInStatus(GameStatusEnum.Auction, out errMsg))
				return false;
			if(!CheckInUserTurn(userId,out errMsg))
				return false;
			//todo pass
			ChangePlayerTurn();
			return true;
		}
	}
}
