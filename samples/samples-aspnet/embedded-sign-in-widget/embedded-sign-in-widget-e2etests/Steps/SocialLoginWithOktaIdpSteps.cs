using embedded_sign_in_widget_e2etests.Helpers;
using embedded_sign_in_widget_e2etests.PageObjectModels;
using FluentAssertions;
using System;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Steps
{
    [Binding]
    public class SocialLoginWithOktaIdpSteps : BaseTestSteps
    {
        private SignInWidgetPage _signInWidgetPage;

        public SocialLoginWithOktaIdpSteps(ITestContext context, SignInWidgetPage signInWidgetPage) : base(context)
        {
            _signInWidgetPage = signInWidgetPage;
        }

        [Given(@"a SPA, WEB APP or MOBILE Policy that defines Password as the only factor required for authentication")]
        public void GivenASPAWEBAPPOrMOBILEPolicyThatDefinesPasswordAsTheOnlyFactorRequiredForAuthentication()
        {
            // no action necessary
        }
        
        [Given(@"a configured IDP object for Okta Idp")]
        public void GivenAConfiguredIDPObjectForOktaIdp()
        {
            // no action necessary
        }
        
        [Given(@"an IDP routing rule defined to allow users in the Sample App to use the IDP")]
        public void GivenAnIDPRoutingRuleDefinedToAllowUsersInTheSampleAppToUseTheIDP()
        {
            // no action necessary
        }
        
        [Given(@"a user named Mary does not have an account in the org but has an Okta Idp account")]
        public void GivenAUserNamedMaryDoesNotHaveAnAccountInTheOrgButHasAnOktaIdpAccount()
        {
            // no action necessary
        }
        
        [When(@"she clicks the Login with Okta Idp button")]
        public void WhenSheClicksTheLoginWithOktaIdpButton()
        {
            _signInWidgetPage.SignonWithOktaIdpButton.Click();
        }
        
        [When(@"logs into Okta Idp application")]
        public void WhenLogsIntoOktaIdpApplication()
        {
            _signInWidgetPage.UserNameInput.SendKeys(_context.Configuration.OktaSocialIdpMfaUserEmail);
            _signInWidgetPage.PasswordInput.SendKeys(_context.Configuration.OktaSocialIdpMfaUserPassword);
            _signInWidgetPage.NextButton.Click();
        }

        [Then(@"she is redirected to the sign in widget view")]
        public void ThenSheIsRedirectedToTheSignInWidgetView()
        {
            _signInWidgetPage.IsAtPath("Account/SignInWidget");
        }

        [Then(@"she sees an option for email authenticator")]
        public void ThenSheSeesAnOptionForEmailAuthenticator()
        {
            _signInWidgetPage.SelectEmailButton.Should().NotBeNull();
        }
    }
}
