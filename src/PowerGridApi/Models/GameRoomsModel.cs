using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PowerGridEngine
{
    public class GameRoomsModel : ApiResponseModel
    {
        public GameRoomModel[] GameRooms { get; set; }
    }
}
