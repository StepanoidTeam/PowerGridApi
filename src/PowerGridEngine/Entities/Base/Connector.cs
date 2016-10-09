using System;

namespace PowerGridEngine
{
    public class Connector: BaseEnergoEntity
    {
        public Map MapRef { get; set; }

        
        public int Cost { get; set; }

        public City City1Ref { get; private set; }

        public City City2Ref { get; private set; }

        private string key;
             
        public string Id
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    var str1 = City2Ref.Id;
                    var str2 = City1Ref.Id;
                    if (string.Compare(City1Ref.Id, City2Ref.Id) < 0)
                    {
                        str1 = City1Ref.Id;
                        str2 = City2Ref.Id;
                    }
                    key = string.Format(Constants.Instance.CONST_CONNECTOR_TEMPLATE, str1, str2);
                }
                return key;
            }
            set
            {
            }
        }
        
        public string City1Key
        {
            get
            {
                if (City1Ref == null)
                    return string.Empty;
                return City1Ref.Id;
            }
            set { }
        }
     
        public string City2Key
        {
            get
            {
                if (City2Ref == null)
                    return string.Empty;
                return City2Ref.Id;
            }
            set { }
        }

        public Connector(int cost, City city1, City city2, Map map = null)
        {
            if (city1 == null || city2 == null)
                throw new ArgumentException("Trying to add undentified cities");
            if (city1.Id == city2.Id)
                throw new ArgumentException("Connector can't connect city to itself");
            Cost = cost;
            City1Ref = city1;
            City2Ref = city2;
            if (map != null)
                map.AddConnector(this);
        }

    }
}
