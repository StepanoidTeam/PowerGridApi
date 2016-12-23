
using System.Collections.Generic;

namespace PowerGridEngine
{
    public class UserModel : BaseEnergoModel<User, UserModelViewOptions>
    {
        public string UserId { get { return Entity.Id; } }

        public string Username { get { return Entity.Username; } }

        public string GameRoomId { get { return Entity.GameRoomRef == null ? null : Entity.GameRoomRef.Id; } }
        
        /// <summary>
        /// Need move it to model like CurrentStageModel (state of active stage)
        /// </summary>
        public bool? ReadyMark
        {
            get
            {
                if (Entity.GameRoomRef == null || !(Entity.GameRoomRef.Stages.Current is CreateGameStage))
                    return null;
                var curStage = Entity.GameRoomRef.Stages.Current as CreateGameStage;
                return curStage.GetReadyMark(UserId);
            }
        }

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
