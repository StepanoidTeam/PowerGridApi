
namespace PowerGridEngine
{
    public class ConnectorModelViewOptions : AbstractModelViewOptions
    {
        public bool Id { get; set; }

        public bool Cost { get; set; }

        public bool CityKeys { get; set; }

        public bool CityNames { get; set; }

        public ConnectorModelViewOptions(bool defaultValue = false)
        {
            Id = defaultValue;
            Cost = defaultValue;
            CityKeys = defaultValue;
            CityNames = defaultValue;
        }
    }
}
