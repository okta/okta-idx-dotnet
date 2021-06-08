using Okta.Idx.Sdk.E2ETests.Helpers;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    public abstract class BaseTestSteps
    {
        protected ITestContext _context;

        public BaseTestSteps(ITestContext context)
        {
            _context = context;
        }
    }
}
