using A18NAdapter;
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
        private IOktaSdkHelper _oktaHelper;

        public TestUserHelper(ITestConfig configuration, IA18nAdapter a18nAdapter, IOktaSdkHelper oktaHelper)
        {
            _configuration = configuration;
            _a18nAdapter = a18nAdapter;
            _oktaHelper = oktaHelper;
        }
        public UserProperties GetActivePasswordUser()
        {
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

        private void CleanUp()
        {

        }

        public void Dispose()
        {
            CleanUp();
        }
    }
}
