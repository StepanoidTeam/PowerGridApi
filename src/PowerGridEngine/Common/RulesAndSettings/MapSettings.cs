
namespace PowerGridEngine
{
    public class MapSettings
    {
        public bool OverrideCityLevelsByRule { get; set; }
        public ICityLevelRule CityLevelRule { get; set; }
    }
}
