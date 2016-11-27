namespace PowerGridApi
{
	public class NetworkRequest
	{
		public string AuthToken { get; set; }

        public NetworkRequestType Type { get; set; }

        public string Data { get; set; }
	}
}
