using Okta.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace embedded_auth_with_sdk.E2ETests.Helpers
{
    public class OktaSdkHelper : IOktaSdkHelper
    {
        private IOktaClient _client;
        private string _resendUri;

        public OktaSdkHelper()
        {
            _client = new OktaClient();
        }

        internal OktaSdkHelper(IOktaClient oktaClient)
        {
            _client = oktaClient;
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

            var user = await _client.Users.CreateUserAsync(options);

            return user;
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
            if (user != default)
            {
                var response = await user.AddFactorAsync(new AddSmsFactorOptions()
                {
                    PhoneNumber = phoneNumber,
                });

                var links = (Dictionary<string, object>)response.GetData()["_links"];
                var resendLink = ((List<object>)links["resend"])[0];

                _resendUri = (string)((Dictionary<string, object>)resendLink)["href"];
                return response;
            }
            return default;
        }

        public async Task AddGoogleAuthenticator(string emailAddress, Func<string, string> sharedSecretToTotpFunc)
        {
            var user = await _client.Users.FirstOrDefaultAsync(u => u.Profile.Email.Equals(emailAddress));
            if (user != default)
            {
                var factor = await user.AddFactorAsync(new AddTotpFactorOptions
                {
                    Provider = FactorProvider.Google,
                });

                var sharedSecret = factor.GetProperty<Resource>("_embedded")
                                        .GetProperty<Resource>("activation")
                                        .GetProperty<string>("sharedSecret");

                await _client.UserFactors.ActivateFactorAsync(
                               new ActivateFactorRequest
                               {
                                   PassCode = sharedSecretToTotpFunc(sharedSecret),
                               },
                               user.Id,
                               factor.Id);

                await AddUserToGroup(user, "MFA required");
            }
        } 

        public async Task ResendEnrollCode()
        {
            if (!string.IsNullOrEmpty(_resendUri))
            {
                await _client.PostAsync(new HttpRequest
                {
                    Uri = _resendUri,
                });
            }
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

            _resendUri = null;
        }
    }
}
