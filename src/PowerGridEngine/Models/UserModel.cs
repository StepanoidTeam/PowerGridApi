
using System.Collections.Generic;

namespace PowerGridEngine
{
    public class UserModel : BaseEnergoModel<User, UserModelViewOptions>
    {
        public string UserId { get { return Entity.Id; } }

        public string Username { get { return Entity.Username; } }

        public string GameRoomId { get { return Entity.GameRoomRef == null ? null : Entity.GameRoomRef.Id; } }

        //weird logic to get ready mark... Why we actially need this PlayerInRoom entity, maybe we could move everything from it
        //to other entities, for example ready mark for sure could be in Player, because player could be (ready) only in ONE room
        public bool? ReadyMark { get { return Entity.GameRoomRef == null ? (bool?)null : Entity.GameRoomRef.Players[UserId].ReadyMark; } }

        public UserModel(User entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(UserModelViewOptions options = null)
        {
            if (options == null)
                options = new UserModelViewOptions(true);

            var result = new Dictionary<string, object>();
            if (options.Id)
                result.Add("Id", this.UserId);
            if (options.Name)
                result.Add("Name", this.Username);
            if (options.GameRoomId)
                result.Add("GameRoomId", this.GameRoomId);
            if (options.ReadyMark)
                result.Add("ReadyMark", this.ReadyMark);

            return result;
        }
    }
}
