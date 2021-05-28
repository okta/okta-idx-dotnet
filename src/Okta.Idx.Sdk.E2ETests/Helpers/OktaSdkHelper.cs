using Okta.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public class OktaSdkHelper : IOktaSdkHelper
    {
        private OktaClient _client;

        public OktaSdkHelper()
        {
            _client = new OktaClient();
        }

        public async Task<IUser> CreateActiveUserIdentifiedWithPassword(string userName, string password)
        {
            //var options = new CreateUserWithImportedHashedPasswordOptions
            //_client.Users.CreateUserAsync()



            return null
        }

    }
}
