using System.Collections.Generic;

namespace PowerGridEngine
{
    public class GameBoardModel : BaseEnergoModel<GameBoard, GameBoardModelViewOptions>
    {
        public GameStatusEnum Status { get; set; }

        public string[] PlayersTurnOrder { get; set; }

		public int? CurrentTurnPlayer { get; set; }

        public IDictionary<string,string> PlayersInfo { get; set; }

        public GameBoardModel(GameBoard entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(GameBoardModelViewOptions options = null)
        {
            if (options == null)
                options = new GameBoardModelViewOptions(true);

            //todo: implement method
            return null;
        }
    }
}
