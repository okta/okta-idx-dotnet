using System;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps
{
    [Binding]
    public class DirectAuthSocialLoginWithMFASteps : BaseTestSteps
    {
        public DirectAuthSocialLoginWithMFASteps(ITestContext context) : base(context)
        { }

        [Given(@"the Application Sign on Policy is set to ""(.*)""")]
        public void GivenTheApplicationSignOnPolicyIsSetTo(string p0)
        { }

        [Given(@"a user named Mary does not have an account in the org")]
        public void GivenAUserNamedMaryDoesNotHaveAnAccountInTheOrg()
        {
            _context.SetMfaOktaSocialIdpUser();
        }
    }
}
