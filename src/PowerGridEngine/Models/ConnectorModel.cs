using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class ConnectorModel : BaseEnergoModel<Connector, ConnectorModelViewOptions>
    {
        public string Id { get { return Entity.Id; } }

        public int Cost { get { return Entity.Cost; } }

        public string City1Key { get { return Entity.City1Key; } }

        public string City2Key { get { return Entity.City2Key; } }

        public string City1Name { get { return Entity.City1Ref.Name; } }

        public string City2Name { get { return Entity.City2Ref.Name; } }

        public ConnectorModel(Connector entity) : base(entity)
        {
        }

        public override Dictionary<string, object> GetInfo(ConnectorModelViewOptions options = null)
        {
            if (options == null)
                options = new ConnectorModelViewOptions(true);

            var result = new Dictionary<string, object>();
            if (options.Id)
                result.Add("Id", this.Id);
            if (options.Cost)
                result.Add("Cost", this.Cost);
            if (options.CityKeys)
            {
                result.Add("City1Key", this.City1Key);
                result.Add("City2Key", this.City2Key);
            }
            if (options.CityNames)
            {
                result.Add("City1Name", this.City1Name);
                result.Add("City2Name", this.City2Name);
            }
            return result;
        }
    }
}
