using Okta.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Drivers
{
    public class OktaSdkDriver
    {
        private OktaClient _client;

        public OktaSdkDriver()
        {
            _client = new OktaClient();
        }

        public async Task CreateUserIdentifiedWithPassword(string userName, string password)
        {

        }

    }
}
