using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps
{
    [Binding]
    public class TOTPSupportGoogleAuthenticatorSteps : BaseTestSteps
    {
        public TOTPSupportGoogleAuthenticatorSteps(ITestContext context) : base(context)
        {
        }

        [Given(@"configured Authenticators are Password \(required\), and Google Authenticator \(required\)")]
        public void GivenConfiguredAuthenticatorsArePasswordRequiredAndGoogleAuthenticatorRequired()
        {
        }

        [Given(@"Mary has an account in the org")]
        public async Task GivenMaryHasAnAccountInTheOrg()
        {
            await _context.SetActiveUserRequiresTotpAsync("Mary");
        }

        [Given(@"Mary has enrolled Google Authenticator already")]
        public async Task GivenMaryHasEnrolledGoogleAuthenticatorAlready()
        {
            await _context.EnrollGoogleAuthenticator();
        }
        
        [Given(@"she is not enrolled in Google Authenticator")]
        public void GivenSheIsNotEnrolledInGoogleAuthenticator()
        {
        }
    }
}
