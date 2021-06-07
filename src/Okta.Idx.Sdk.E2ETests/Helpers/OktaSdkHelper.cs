using Okta.Sdk;
using System.Linq;
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

        public async Task DeleteUserAsync(string email)
        {
            var user = await _client.Users.FirstOrDefaultAsync(u => u.Profile.Email.Equals(email));
            if (user != default)
            {
                await user.DeactivateAsync();
                await user.DeactivateOrDeleteAsync();
            }
        }

        public async Task<IUser> CreateActiveUser(string userName, string phone, string firstName, string password)
        {
            var options = new CreateUserWithPasswordOptions
            {
                Profile = new Okta.Sdk.UserProfile
                {
                    Email = userName,
                    Login = userName,
                    FirstName = firstName,
                    LastName = "Lastname",
                    PrimaryPhone = phone,
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

        public async Task<IUserFactor> EnrollPhoneFactor(string emailAddress, string phoneNumber)
        {
            var user = await _client.Users.FirstOrDefaultAsync(u => u.Profile.Email.Equals(emailAddress));
            if (user!=default)
            {
                return await user.AddFactorAsync(new AddSmsFactorOptions()
                {
                    PhoneNumber = phoneNumber,
                });
            }
            return default;
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
