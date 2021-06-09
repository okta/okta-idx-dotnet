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

        public TestUserProfile TestUserProfile { get; set; }

        public TestContext(ITestConfiguration configuration, IOktaSdkHelper oktaHelper)
        {
            _oktaHelper = oktaHelper;
            _configuration = configuration;

            TestUserProfile = new TestUserProfile();
        }

        public async Task SetActivePasswordUserAsync(string firstName)
        {
            await _oktaHelper.DeleteUserAsync(_configuration.UserName);

            var oktaUser = await _oktaHelper.CreateActiveUser(_configuration.UserName, firstName, _configuration.UserPassword);

            TestUserProfile = new TestUserProfile()
            {
                FirstName = firstName,
                LastName = "Lastname",
                Email = oktaUser.Profile.Email,
                Password = _configuration.UserPassword
            };
        }

    }
}
