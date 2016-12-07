namespace PowerGridApi
{
	public class DuplexNetworkRequest
    {
		public string AuthToken { get; set; }

        public DuplexNetworkRequestType Type { get; set; }

        public string Data { get; set; }
	}
}
