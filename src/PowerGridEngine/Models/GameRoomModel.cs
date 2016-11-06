using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class GameRoomModel : BaseEnergoModel<GameRoom, RoomModelViewOptions>
    {
        public string Id { get { return Entity.Id; } }

        public string Name { get { return Entity.Name; } }

        public bool IsInGame { get { return Entity.IsInGame; } }

        public int UserCount { get { return Entity.Players == null ? 0 : Entity.Players.Count(); } }
        
        public object[] GetUsersDetails(UserModelViewOptions viewOptions)
        {
            return Entity.Players == null ? new object[0] : Entity.Players.Select(m => new UserModel(m.Value.Player).GetInfo(viewOptions)).ToArray();
        }

        public GameRoomModel(GameRoom entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(RoomModelViewOptions options = null)
        {
            if (options == null)
                options = new RoomModelViewOptions(true);

            var result = new Dictionary<string, object>();
            if (options.Id)
                result.Add("Id", this.Id);
            if (options.Name)
                result.Add("Name", this.Name);
            if (options.IsInGame)
                result.Add("IsInGame", this.IsInGame);
            if (options.UserCount)
                result.Add("UserCount", this.UserCount);
            if (options.UserDetails)
                result.Add("UserDetails", this.GetUsersDetails(options.UserViewOptions));

            return result;
        }
    }
}
