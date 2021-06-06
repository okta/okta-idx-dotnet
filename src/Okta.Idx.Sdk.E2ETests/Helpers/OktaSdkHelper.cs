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

        public async Task<IUser> CreateActiveUserAndAddToGroupAsync(string userName, string firstName, string password, string groupName)
        {
            IUser user = await CreateActiveUser(userName, firstName, password);

            var groupList = await _client.Groups.ListGroups(groupName).ToArrayAsync();
            var group = groupList.Single();

            await _client.Groups.AddUserToGroupAsync(group.Id, user.Id);

            return user;
        }

        public async Task<IUser> CreateActiveUserIdentifiedWithPasswordAsync(string email, string firstName, string password)
        {
            return await CreateActiveUser(email, firstName, password);
        }

        public Task<IUser> CreateUnassignedUserIdentifiedWithPasswordAsync(string userName, string password)
        {
            throw new NotImplementedException();
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

        private async Task<IUser> CreateActiveUser(string userName, string firstName, string password)
        {
            var options = new CreateUserWithPasswordOptions
            {
                Profile = new UserProfile
                {
                    Email = userName,
                    Login = userName,
                    FirstName = firstName,
                    LastName = "Lastname",
                },
                Password = password,
                Activate = true,
            };

            return await _client.Users.CreateUserAsync(options);
        }

    }
}
