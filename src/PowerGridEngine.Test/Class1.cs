using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PowerGridEngine.Test
{
    public class Class1
    {
        [Fact]
        public void TestLogin()
        {
            var errMsg = string.Empty;
            var result = EnergoServer.Current.Login("TestUsername", out errMsg);
            Assert.NotEqual(result, null);
            Assert.NotEqual(result.AuthToken, null);
            Assert.NotEqual(result.Id, null);
            Assert.NotEqual(result.Username, "TestUsername");
        }

        [Fact]
        public void TestLoginWithSpaces()
        {
            var errMsg = string.Empty;
            var result = EnergoServer.Current.Login("Test Username", out errMsg);
            Assert.NotEqual(result, null);
            Assert.NotEqual(result.AuthToken, null);
            Assert.NotEqual(result.Id, null);
            Assert.NotEqual(result.Username, "TestUsername");
        }
    }
}
