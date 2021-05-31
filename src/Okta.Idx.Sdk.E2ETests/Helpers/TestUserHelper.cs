using A18NAdapter;
using A18NAdapter.Dto;
using System;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public class TestUserHelper : ITestUserHelper, IDisposable
    {
        private IA18nAdapter _a18nAdapter;
        private ITestConfig _configuration;
        private A18nProfile _a18nProfile;
        private IOktaSdkHelper _oktaHelper;
       // private IUser oktaUser;

        public TestUserHelper(ITestConfig configuration, IA18nAdapter a18nAdapter, IOktaSdkHelper oktaHelper)
        {
            _oktaHelper = oktaHelper;
            _a18nAdapter = a18nAdapter;
            _configuration = configuration;

            _a18nAdapter.SetDefaultProfileId(configuration.A18nProfileId);
            _a18nProfile = _a18nAdapter.GetProfileAsync().Result;
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

        private async Task CleanUpA18ProfileAsync()
        {
            await _a18nAdapter.DeleteAllProfileEmailsAsync();
            await _a18nAdapter.DeleteAllProfileSmsAsync();
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
            CleanUpOktaUserAsync().Wait();
        }
    }
}
