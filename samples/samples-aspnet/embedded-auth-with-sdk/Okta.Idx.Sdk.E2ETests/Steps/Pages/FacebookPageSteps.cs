using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class FacebookPageSteps: BaseTestSteps
    {
        private FacebookLoginPage _facebookLoginPageModel;

        public FacebookPageSteps(ITestContext context, FacebookLoginPage facebookLoginPageModel)
            : base(context)
        {
            _facebookLoginPageModel = facebookLoginPageModel;
        }

        [When(@"logs in to Facebook")]
        public void WhenLogsInToFacebook()
        {
            _facebookLoginPageModel.UserNameInput.SendKeys(_context.UserProfile.Email);
            _facebookLoginPageModel.PasswordInput.SendKeys(_context.UserProfile.Password);
            _facebookLoginPageModel.LoginButton.Click();
        }
    }
}
