namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    public class BasePageSteps
    {
        protected ITestConfig _testConfig;

        public BasePageSteps(ITestConfig testConfig)
        {
            _testConfig = testConfig;
        }
    }
}