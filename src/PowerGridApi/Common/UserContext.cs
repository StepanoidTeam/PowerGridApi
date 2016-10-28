using System;

namespace PowerGridApi
{
    public class UserContext
    {
        public string AuthToken { get; private set; }

        public UserContext(string authToken)
        {
            AuthToken = authToken;
        }
    }
}
