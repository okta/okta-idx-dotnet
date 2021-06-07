using Okta.Idx.Sdk.E2ETests.Helpers;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    public abstract class BaseTestSteps
    {
        protected TestUserProperties _testUser;
        protected ITestUserHelper _userHelper;

        public BaseTestSteps(ITestUserHelper userHelper)
        {
            _userHelper = userHelper;
        }
    }
}
