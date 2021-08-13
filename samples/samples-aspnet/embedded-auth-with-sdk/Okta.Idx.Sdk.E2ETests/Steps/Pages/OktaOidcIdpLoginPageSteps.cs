using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class OktaOidcIdpLoginPageSteps: BaseTestSteps
    {
        private OktaOidcIdpLoginPage _oktaOidcIdpLoginPageModel;

        public OktaOidcIdpLoginPageSteps(ITestContext context, OktaOidcIdpLoginPage oktaOidcIdpLoginPageModel)
            : base(context)
        {
            _oktaOidcIdpLoginPageModel = oktaOidcIdpLoginPageModel;
        }

        [When(@"logs in to Okta")]
        public void WhenLogsInToOkta()
        {
            _oktaOidcIdpLoginPageModel.UserNameInput.SendKeys(_context.UserProfile.Email);
            _oktaOidcIdpLoginPageModel.PasswordInput.SendKeys(_context.UserProfile.Password);
            _oktaOidcIdpLoginPageModel.LoginButton.Click();
        }
    }
}
