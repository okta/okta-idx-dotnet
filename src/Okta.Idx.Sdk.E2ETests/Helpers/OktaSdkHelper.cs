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

        public async Task<IUser> CreateActiveUserIdentifiedWithPasswordAsync(string email, string password)
        {
            var options = new CreateUserWithPasswordOptions
            {
                Profile = new UserProfile
                {
                    Email = email,
                    Login = email,
                    FirstName = "Mary",
                    LastName = "Lastname",
                },
                Password = password,
                Activate = true,
            };

            return await _client.Users.CreateUserAsync(options);
        }

        public async Task DeleteUserAsync(string email)
        {
            var user = await _client.Users.FirstOrDefaultAsync(u => u.Profile.Email.Equals(email));
            if (user != default)
            {
                await user.DeactivateAsync();
                await user.DeactivateOrDeleteAsync();
            }
        }
    }
}
