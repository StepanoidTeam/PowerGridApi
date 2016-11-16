
using System.ComponentModel.DataAnnotations;

namespace PowerGridEngine
{
    public class CreateRoomModel
    {
        public string Name { get; set; }

        public bool SetReadyMark { get; set; }

        public CreateRoomModel()
        {
            SetReadyMark = true;
        }
    }
}
