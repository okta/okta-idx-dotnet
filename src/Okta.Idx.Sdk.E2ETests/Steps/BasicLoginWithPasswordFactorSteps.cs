using Okta.Idx.Sdk.E2ETests.Helpers;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class BasicLoginWithPasswordFactorSteps : BaseTestSteps
    {

        public BasicLoginWithPasswordFactorSteps(ITestUserHelper userHelper, 
                                                ITestConfig testConfig)
            : base(userHelper, testConfig)
        { }

        [Given(@"a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required")]
        public void GivenASPAWEBAPPOrMOBILESignOnPolicyThatDefinesPasswordAsRequired()
        { }

        [Given(@"the list of Authenticators contains Email and Password")]
        public void GivenTheListOfAuthenticatorsContainsEmailAndPassword()
        { }

        [Given(@"a User named ""(.*)"" exists, and this user has already setup email and password factors")]
        public async Task GivenAUserNamedExistsAndThisUserHasAlreadySetupEmailAndPasswordFactors(string userName)
        {
            _testConfig.TestUser = await _userHelper.GetActivePasswordUserAsync(userName);
        }
    }
}
