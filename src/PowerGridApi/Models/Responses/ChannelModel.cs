using PowerGridEngine;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridApi
{
    public class ChannelModel : BaseEnergoModel<ChatChannel, ChannelModelViewOptions>
    {
        public string Id { get { return Entity.Id; } }

        public string Name { get { return Entity.Name; } }

        public string Type { get { return Entity.Type.ToString(); } }

        public bool IsJoined { get; private set; }

        public ChannelModel(ChatChannel entity, bool isJoined) : base(entity)
        {
            IsJoined = isJoined;
        }

        public override Dictionary<string, object> GetInfo(ChannelModelViewOptions options = null)
        {
            if (options == null)
                options = new ChannelModelViewOptions(true);

            var result = new Dictionary<string, object>();
            if (options.Id)
                result.Add("Id", this.Id);
            if (options.Name)
                result.Add("Name", this.Name);
            if (options.Type)
                result.Add("Type", this.Type);
            if (options.IsJoinedOrInvited)
            {
                result.Add("IsJoined", this.IsJoined);
                result.Add("IsInvited", !this.IsJoined);
            }

            return result;
        }
    }
}
