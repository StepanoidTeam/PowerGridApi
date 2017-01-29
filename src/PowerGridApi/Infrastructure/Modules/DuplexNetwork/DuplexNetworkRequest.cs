using PowerGridEngine;

namespace PowerGridApi
{
	public class DuplexNetworkRequest: IWebSocketRequestModel
    {
		public string AuthToken { get; set; }

        public DuplexNetworkRequestType Type { get; set; }

	}
}
