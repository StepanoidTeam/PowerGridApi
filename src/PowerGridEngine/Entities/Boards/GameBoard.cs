﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
	
	public class GameBoard : BaseEnergoEntity
	{
		public override BaseEnergoViewModel ToViewModel(IViewModelOptions options = null)
		{
			var ret = new GameBoardViewModel();
			ret.Status = this.Status;
			ret.PlayersTurnOrder = this.PlayersTurnOrder;
			ret.CurrentTurnPlayer = this.CurrentTurnPlayer;
			ret.PlayersInfo = context.PlayerBoards.ToDictionary(n => n.Key, m => m.Value.Money.ToString());
			return ret;
		}

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
				errMsg = string.Format(Constants.Instance.CONST_ERR_MSG_ALREADY_IN_THE_GAME, status.ToString());
			return this.Status == status;
		}

		public bool CheckInUserTurn(string userId, out string errMsg)
		{
			errMsg = string.Empty;
			if (this.PlayersTurnOrder[this.CurrentTurnPlayer] == userId)
				return true;
			var p = context.PlayerBoards.FirstOrDefault(m => m.Key == userId);
			errMsg = string.Format(Constants.Instance.CONST_ERR_MSG_IS_NOT_YOUR_TURN, p.Value.PlayerRef.Username);
			return false;
		}

		public IEnumerable<GameActionEnum> GetAllowedActions(string userId, out string errMsg)
		{
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
