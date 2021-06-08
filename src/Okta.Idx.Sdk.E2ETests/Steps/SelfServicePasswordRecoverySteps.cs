using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class SelfServicePasswordRecoverySteps : BaseTestSteps
    {

        public SelfServicePasswordRecoverySteps(ITestContext context)
            : base(context)
        { }

        [Given(@"an org with an ORG Policy that defines Authenticators with Password and Email as required")]
        public void GivenAnOrgWithAnORGPolicyThatDefinesAuthenticatorsWithPasswordAndEmailAsRequired()
        { }

        [Given(@"a user named ""(.*)""")]
        public async Task GivenAUserNamed(string firstName)
        {
            await _context.SetActivePasswordUserAsync(firstName);
        }

        [Given(@"Mary is a user with a verified email and a set password")]
        public void GivenMaryIsAUserWithAVerifiedEmailAndASetPasswordAsync()
        { }

    }
}
