using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class RootPageForDirectAuthDemoApplicationSteps : BaseTestSteps
    {
        public RootPageForDirectAuthDemoApplicationSteps(ITestContext context)
            : base(context)
        {
        }

        [Given(@"Mary has an UNauthenticated session")]
        public async Task GivenMaryHasAnUNauthenticatedSession()
        {
            await _context.SetActivePasswordUserAsync("Mary");

        }

        [Given(@"Mary has an authenticated session")]
        public void GivenMaryHasAnAuthenticatedSession()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"Mary navigates to the Root View")]
        public void GivenMaryNavigatesToTheRootView()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"Mary navigates to the Root View")]
        public void WhenMaryNavigatesToTheRootView()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"Mary clicks the logout button")]
        public void WhenMaryClicksTheLogoutButton()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the Root Page shows links to the Entry Points as defined in https://oktawiki\.atlassian\.net/l/c/Pw(.*)DVm(.*)t")]
        public void ThenTheRootPageShowsLinksToTheEntryPointsAsDefinedInHttpsOktawiki_Atlassian_NetLCPwDVmt(int p0, int p1)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"Mary sees a table with the claims from the /userinfo response")]
        public void ThenMarySeesATableWithTheClaimsFromTheUserinfoResponse()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"Mary sees a logout button")]
        public void ThenMarySeesALogoutButton()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she is redirected back to the Root View")]
        public void ThenSheIsRedirectedBackToTheRootView()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"Mary sees login, registration buttons")]
        public void ThenMarySeesLoginRegistrationButtons()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she does not see claims from /userinfo")]
        public void ThenSheDoesNotSeeClaimsFromUserinfo()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
