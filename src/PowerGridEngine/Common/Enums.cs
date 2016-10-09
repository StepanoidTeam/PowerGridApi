
namespace PowerGridEngine
{
	
	public enum GameActionEnum
	{
		/// <summary>
		/// Спасовать выбирать карту на ауке
		/// </summary>
		//[Description(Name = "Pass")]
		AuctionPass,

		/// <summary>
		/// Выбор карты на ауке
		/// </summary>
		//[Display(Name = "Select Card")]
		AuctionSelectCard,

		/// <summary>
		/// Поднять ставку на станцию на ауке
		/// </summary>
		//[Display(Name = "Raise Bet")]
		AuctionRaise,

		/// <summary>
		/// Пропустить текущее предложение на ауке
		/// </summary>
		//[Display(Name = "Skip Bet")]
		AuctionSkip
	}

    
    public enum GameStatusEnum
    {
        Auction,
        
        RecourcesTrading,
        
        CitiesTrading,
        
        Bureaucracy
    }
}
