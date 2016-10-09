
namespace PowerGridEngine
{
    public class RegionModelViewOptions : AbstractIdNameModelViewOptions
    {
        public bool IsLocked { get; set; }

        public bool Cities { get; set; }

        public CityModelViewOptions CityViewOptions { get; set; }

        public RegionModelViewOptions(bool defaultValue = false): base(defaultValue)
        {
            IsLocked = defaultValue;
            Cities = defaultValue;
            CityViewOptions = new CityModelViewOptions(defaultValue);
        }
    }
}
