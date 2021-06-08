using TechTalk.SpecFlow;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class MfaWithPasswordAndEmailSteps : BaseTestSteps
    {
        public MfaWithPasswordAndEmailSteps(ITestContext context)
            : base(context)
        { }

        [Given(@"a SPA, WEB APP or MOBILE Policy that defines MFA with Password and Email as required")]
        public void GivenASPAWEBAPPOrMOBILEPolicyThatDefinesMFAWithPasswordAndEmailAsRequired()
        { }

        [Given(@"a User named ""(.*)"" created in the admin interface with a Password only")]
        public async Task GivenAUserNamedCreatedInTheAdminInterfaceWithAPasswordOnly(string firstName)
        {
            await _context.SetActivePasswordAndEmailUserAsync(firstName);
        }
    }
}
