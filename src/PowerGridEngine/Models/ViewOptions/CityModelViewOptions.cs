
namespace PowerGridEngine
{
    public class CityModelViewOptions : AbstractIdNameModelViewOptions
    {
        public bool RegionKey { get; set; }

        public bool RegionName { get; set; }

        public bool Coords { get; set; }

        public bool Levels { get; set; }

        public bool Conntectors { get; set; }

        public ConnectorModelViewOptions ConnectorViewOptions { get; set; }    

        public CityModelViewOptions(bool defaultValue = false): base(defaultValue)
        {
            RegionKey = defaultValue;
            RegionName = defaultValue;
            Coords = defaultValue;
            Levels = defaultValue;
            Conntectors = defaultValue;
            ConnectorViewOptions = new ConnectorModelViewOptions(defaultValue);
        }
    }
}
