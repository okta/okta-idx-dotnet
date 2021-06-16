using embedded_sign_in_widget_e2etests.Helpers;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Steps
{
    [Binding]
    public class DirectAuthWithSelfHostedSignInWidgetSteps : BaseTestSteps
    {

        public DirectAuthWithSelfHostedSignInWidgetSteps(ITestContext context)
            : base(context)
        {
        }
    
        [Given(@"a SPA, WEB APP or MOBILE Policy that defines Password as the only factor required for authentication")]
        public void GivenASPAWEBAPPOrMOBILEPolicyThatDefinesPasswordAsTheOnlyFactorRequiredForAuthentication()
        { }
        
        [Given(@"a configured IDP object for Google")]
        public void GivenAConfiguredIDPObjectForGoogle()
        { }
        
        [Given(@"an IDP routing rule defined to allow users in the Sample App to use the IDP")]
        public void GivenAnIDPRoutingRuleDefinedToAllowUsersInTheSampleAppToUseTheIDP()
        { }
        
        [Given(@"a user named Mary does not have an account in the org")]
        public void GivenMaryDoesNotHaveAnAccountInTheOrg()
        {
            _context.SetUnenrolledUserWithFacebookAccount();
        }
        
        [Then(@"the Widget is reinitialized to re-enter the remediation flow to complete authentication")]
        public void ThenTheWidgetIsReinitializedToRe_EnterTheRemediationFlowToCompleteAuthentication()
        { }
    }
}
