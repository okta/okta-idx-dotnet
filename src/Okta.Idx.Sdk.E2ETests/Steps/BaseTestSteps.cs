using Okta.Idx.Sdk.E2ETests.Helpers;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    public abstract class BaseTestSteps
    {
        protected ITestUserHelper _userHelper;
        protected ITestConfig _testConfig;

        public BaseTestSteps(ITestUserHelper userHelper, ITestConfig testConfig)
        {
            _userHelper = userHelper;
            _testConfig = testConfig;
        }
    }
}
