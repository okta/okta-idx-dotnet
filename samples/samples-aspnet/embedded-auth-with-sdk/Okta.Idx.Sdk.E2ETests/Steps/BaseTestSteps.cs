using embedded_auth_with_sdk.E2ETests.Helpers;

namespace embedded_auth_with_sdk.E2ETests.Steps
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
