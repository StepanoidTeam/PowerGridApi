
namespace PowerGridEngine
{
    public class MapModelViewOptions : AbstractModelViewOptions
    {    
        public bool Cities { get; set; }

        public bool Regions { get; set; }
       
        public bool Connectors { get; set; }

        public CityModelViewOptions CityViewOptions { get; set; }

        public ConnectorModelViewOptions ConnectorViewOptions { get; set; }

        public RegionModelViewOptions RegionViewOptions { get; set; }
        
        public MapModelViewOptions(bool defaultValue = false)
        {
            Cities = defaultValue;
            Regions = defaultValue;
            Connectors = defaultValue;
            CityViewOptions = new CityModelViewOptions(defaultValue);
            ConnectorViewOptions = new ConnectorModelViewOptions(defaultValue);
            RegionViewOptions = new RegionModelViewOptions(defaultValue);
        }
    }
}
