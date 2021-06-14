using embedded_sign_in_widget_e2etests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Steps
{
    [Binding]
    public class BasicLoginWithPasswordFactorSteps : BaseTestSteps
    {

        public BasicLoginWithPasswordFactorSteps(ITestContext context)
            : base(context)
        {
        }

        [Given(@"a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required")]
        [Given(@"the list of Authenticators contains Email and Password")]
        public void GivenASPAWEBAPPOrMOBILESignOnPolicyThatDefinesPasswordAsRequired()
        { }

        [Given(@"a User named Mary exists, and this user has already setup email and password factors")]
        public async Task GivenAUserNamedExistsAndThisUserHasAlreadySetupEmailAndPasswordFactors()
        {
            await _context.SetActivePasswordUserAsync();
        }
    }
}
