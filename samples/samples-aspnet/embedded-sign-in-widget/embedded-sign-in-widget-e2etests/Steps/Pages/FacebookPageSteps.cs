using embedded_sign_in_widget_e2etests.Helpers;
using embedded_sign_in_widget_e2etests.PageObjectModel;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Steps.Pages
{
    [Binding]
    public class FacebookPageSteps : BaseTestSteps
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
            _facebookLoginPageModel.UserNameInput.SendKeys(_context.TestUserProfile.Email);
            _facebookLoginPageModel.PasswordInput.SendKeys(_context.TestUserProfile.Password);
            _facebookLoginPageModel.LoginButton.Click();
        }
    }
}
