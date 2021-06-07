using TechTalk.SpecFlow;
using Okta.Idx.Sdk.E2ETests.Helpers;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class MfaWithPasswordAndEmailSteps : BaseTestSteps
    {
        public MfaWithPasswordAndEmailSteps(ITestUserHelper userHelper,
                                            ITestConfig testConfig)
            : base(userHelper, testConfig)
        { }

        [Given(@"a SPA, WEB APP or MOBILE Policy that defines MFA with Password and Email as required")]
        public void GivenASPAWEBAPPOrMOBILEPolicyThatDefinesMFAWithPasswordAndEmailAsRequired()
        {
        }

        [Given(@"a User named ""(.*)"" created in the admin interface with a Password only")]
        public async Task GivenAUserNamedCreatedInTheAdminInterfaceWithAPasswordOnly(string firstName)
        {
            _testConfig.TestUser = await _userHelper.GetActivePasswordAndEmailUserAsync(firstName);
        }

        [Given(@"her password is correct")]
        public void GivenHerPasswordIsCorrect()
        {
        }
    }
}
