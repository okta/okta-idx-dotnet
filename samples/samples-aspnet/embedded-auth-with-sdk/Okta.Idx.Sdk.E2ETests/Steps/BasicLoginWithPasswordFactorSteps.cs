using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps
{
    [Binding]
    public class BasicLoginWithPasswordFactorSteps : BaseTestSteps
    {

        public BasicLoginWithPasswordFactorSteps(ITestContext context)
            : base(context)
        { }

        [Given(@"a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required")]
        [Given(@"the list of Authenticators contains Email and Password")]
        public void GivenASPAWEBAPPOrMOBILESignOnPolicyThatDefinesPasswordAsRequired()
        { }

        [Given(@"a User named ""(.*)"" exists, and this user has already setup email and password factors")]
        public async Task GivenAUserNamedExistsAndThisUserHasAlreadySetupEmailAndPasswordFactors(string userName)
        {
            await _context.SetActivePasswordUserAsync(userName);
        }

        [Given(@"a User named ""(.*)"" exists, and this user has already setup email, phone and password factors")]
        public async Task GivenAUserNamedExistsAndThisUserHasAlreadySetupEmailAndPhoneAndPasswordFactors(string userName)
        {
            await _context.SetActivePasswordAndSmsUserAsync(userName);
            await _context.EnrollPhoneAuthenticator();
        }
    }
}
