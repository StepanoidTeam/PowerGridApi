
namespace PowerGridEngine
{
    public class ChatSendModel : IWebSocketRequestModel
    {
        public string To { get; set; }

        public bool InRoomChannel { get; set; }

        public string Message { get; set; }
    }
}
