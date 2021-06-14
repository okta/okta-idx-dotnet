using Okta.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Helpers
{
    public class TestContext : ITestContext
    {
        private readonly ITestConfiguration _configuration;
        private readonly IOktaSdkHelper _oktaHelper;
        private bool _disposed = false;
        public TestUserProfile TestUserProfile { get; set; }

        public TestContext(ITestConfiguration configuration, IOktaSdkHelper oktaHelper)
        {
            _oktaHelper = oktaHelper;
            _configuration = configuration;

            TestUserProfile = new TestUserProfile();
        }

        public async Task SetActivePasswordUserAsync()
        {
            // create user dynamically +Guid
            var guid = Guid.NewGuid();
            var username = $"mary-embedded-siw-{guid}@example.com";
            var firstName = $"Mary";
            var password = "P4zzw0rd1";
            

            var oktaUser = await _oktaHelper.CreateActiveUser(username, firstName, password);

            TestUserProfile = new TestUserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                Password = password
            };
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
                _oktaHelper.DeleteUserAsync(TestUserProfile.Email).Wait();
            }
            _disposed = true;
        }
    }
}
