
namespace PowerGridEngine
{
    public class RoomLookupSettings
    {       
        public string Id { get; set; }

        /// <summary>
        /// In case is true - will ignore search by Id
        /// </summary>
        public bool CurrentPlayerInside { get; set; }

        public RoomLookupSettings()
        {
        }
    }
}
