using A18NAdapter;
using A18NAdapter.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public class TestUserHelper : ITestUserHelper, IDisposable
    {
        private IA18nAdapter _a18nAdapter;
        private ITestConfig _configuration;
        private A18nProfile _a18nProfile;
        private IOktaSdkHelper _oktaHelper;

        public TestUserHelper(ITestConfig configuration, IA18nAdapter a18nAdapter, IOktaSdkHelper oktaHelper)
        {
            _oktaHelper = oktaHelper;
            _a18nAdapter = a18nAdapter;
            _configuration = configuration;

            _a18nProfile = _a18nAdapter.CreateProfileAsync().Result;
            _a18nAdapter.SetDefaultProfileId(_configuration.A18nProfileId);
            CleanUp().Wait();
        }
        public async Task<UserProperties> GetActivePasswordUser()
        {
            var user = await _oktaHelper.CreateActiveUserIdentifiedWithPassword(_a18nProfile.EmailAddress, _configuration.UserPassword);


            return new UserProperties()
            {
                Email = _configuration.NormalUser,
                Password = _configuration.UserPassword
            };
        }

        public UserProperties GetUnassignedUser()
        {
            return new UserProperties()
            {
                Email = _configuration.UnassignedUser,
                Password = _configuration.UserPassword
            };
        }

        private async Task CleanUp()
        {
            await _a18nAdapter.DeleteAllProfileEmailsAsync(profileId: default);
            await _a18nAdapter.DeleteAllProfileSmsAsync(profileId: default);
            await 
        }

        public void Dispose()
        {
        }
    }
}
