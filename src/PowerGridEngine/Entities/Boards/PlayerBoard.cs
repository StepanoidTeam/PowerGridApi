
namespace PowerGridEngine
{
    
    public class PlayerBoard
    {
        private GameContext context { get; set; }

        public int Money { get; private set; }
        
        public Player PlayerRef { get; private set; }

        public PlayerBoard(GameContext context, Player player)
        {
            this.context = context;
            PlayerRef = player;
        }

        public bool CanMakePayment(int charge)
        {
            if (charge >= 0)
                return true;
            return (Money + charge) >= 0;
        }

        public bool Payment(int charge)
        {
            if (!CanMakePayment(charge))
                return false;
            Money += charge;
            return true;
        }
    }
}
