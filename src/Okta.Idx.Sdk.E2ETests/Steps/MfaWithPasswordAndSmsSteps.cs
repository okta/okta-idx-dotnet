using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class MfaWithPasswordAndSmsSteps : BaseTestSteps
    {
        public MfaWithPasswordAndSmsSteps(ITestContext context)
                : base(context)
        { }

        [Given(@"a SPA, WEB APP or MOBILE Policy that defines MFA with Password and SMS as required")]
        [Given(@"an Authenticator Enrollment Policy that has PHONE as optional and EMAIL as required for the Everyone Group")] 
        public void GivenASPAWEBAPPOrMOBILEPolicyThatDefinesMFAWithPasswordAndSMSAsRequired()
        { }

        [Given(@"a User named ""(.*)"" created that HAS NOT yet enrolled in the SMS factor")]
        public async Task GivenAUserNamedCreatedThatHASNOTYetEnrolledInTheSMSFactor(string firstName)
        {
            await _context.SetActivePasswordAndSmsUserAsync(firstName);
        }

        [Given(@"she has enrolled her phone already")]
        public async Task GivenSheHasEnrolledHerPhoneAlready()
        {
            await _context.EnrollPhoneAuthenticator();
        }

        [Then(@"she should see a message ""(.*)""")]
        public void ThenSheShouldSeeAMessage(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the sample show as error message ""(.*)"" on the SMS Challenge page")]
        public void ThenTheSampleShowAsErrorMessageOnTheSMSChallengePage(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she sees a field to re-enter another code")]
        public void ThenSheSeesAFieldToRe_EnterAnotherCode()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
