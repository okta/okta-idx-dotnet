using Okta.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Helpers
{
    public class OktaSdkHelper : IOktaSdkHelper
    {
        private OktaClient _client;

        public OktaSdkHelper()
        {
            _client = new OktaClient();
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

        public async Task<IUser> CreateActiveUser(string userName, string firstName, string password)
        {
            var options = new CreateUserWithPasswordOptions
            {
                Profile = new UserProfile
                {
                    Email = userName,
                    Login = userName,
                    FirstName = firstName,
                    LastName = "Lastname",
                    PrimaryPhone = "2065551212",
                },
                Password = password,
                Activate = true,
            };

            return await _client.Users.CreateUserAsync(options);
        }

        public async Task AddUserToGroup(IUser user, string groupName)
        {
            var groupList = await _client.Groups.ListGroups(groupName).ToArrayAsync();
            var group = groupList.Single();

            await _client.Groups.AddUserToGroupAsync(group.Id, user.Id);
        }

        public async Task ActivateFactor(IUserFactor factor, string emailAddress, string passCode)
        {
            var user = await _client.Users.FirstOrDefaultAsync(u => u.Profile.Email.Equals(emailAddress));
            if (user != default)
            {
                await factor.ActivateAsync(new ActivateFactorRequest
                {
                    PassCode = passCode,
                },
                user.Id);
            }
        }
    }
}
