using A18NClient;
using A18NClient.Dto;
using System;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public class TestUserHelper : ITestUserHelper, IDisposable
    {
        private IA18nClient _a18nClient;
        private ITestConfig _configuration;
        private A18nProfile _a18nProfile;
        private IOktaSdkHelper _oktaHelper;
        private bool _disposed = false;


        public TestUserHelper(ITestConfig configuration, IA18nClient a18nClient, IOktaSdkHelper oktaHelper)
        {
            _oktaHelper = oktaHelper;
            _a18nClient = a18nClient;
            _configuration = configuration;

            _a18nClient.SetDefaultProfileId(configuration.A18nProfileId);
            _a18nProfile = _a18nClient.GetProfileAsync().Result;
        }

        public async Task<TestUserProperties> GetActivePasswordUserAsync()
        {
            await CleanUpAsync();

            var oktaUser = await _oktaHelper.CreateActiveUserIdentifiedWithPasswordAsync(_a18nProfile.EmailAddress, _configuration.UserPassword);
            
            return new TestUserProperties()
            {
                Email = oktaUser.Profile.Email,
                PhoneNumber = oktaUser.Profile.PrimaryPhone,        
                Password = _configuration.UserPassword,
            };
        }

        public TestUserProperties GetUnassignedUser()
        {
            return new TestUserProperties()
            {
                Email = _configuration.UnassignedUser,
                Password = _configuration.UserPassword
            };
        }

        public async Task<string> GetRecoveryCodeFromEmail()
        {
            for (int tries = 0; tries < 30; tries++) {
                var mail = await _a18nClient.GetLatestEmailMessageAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    return extractRecoveryCodeFromEmail(content);
                }
                await Task.Delay(1000);    
            }

            return string.Empty;
        }

        private string extractRecoveryCodeFromEmail(string content)
        {
            throw new NotImplementedException();
        }

        private async Task CleanUpA18ProfileAsync()
        {
            await _a18nClient.DeleteAllProfileEmailsAsync();
            await _a18nClient.DeleteAllProfileSmsAsync();
        }

        private async Task CleanUpOktaUserAsync()
        {
            await _oktaHelper.DeleteUserAsync(_a18nProfile.EmailAddress);
        }

        private async Task CleanUpAsync()
        {
            await CleanUpA18ProfileAsync();
            await CleanUpOktaUserAsync();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                CleanUpOktaUserAsync().Wait();
            }
            _disposed = true;
        }

    }
}
