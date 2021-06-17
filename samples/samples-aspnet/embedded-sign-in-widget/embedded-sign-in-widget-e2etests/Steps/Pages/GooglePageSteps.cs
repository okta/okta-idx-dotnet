using embedded_sign_in_widget_e2etests.Helpers;
using embedded_sign_in_widget_e2etests.PageObjectModels;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Steps.Pages
{
    [Binding]
    public class GooglePageSteps : BaseTestSteps
    {
        private GoogleLoginPage _googleLoginPageModel;

        public GooglePageSteps(ITestContext context, GoogleLoginPage googleLoginPageModel)
            : base(context)
        {
            _googleLoginPageModel = googleLoginPageModel;
        }

        [When(@"logs in to Google")]
        public void WhenLogsInToGoogle()
        {
            _googleLoginPageModel.UserNameInput.TrySendKeys($"{_context.TestUserProfile.Email}");
            _googleLoginPageModel.UserNameInput.SendKeys(Keys.Enter);

            _googleLoginPageModel.PasswordInput.TrySendKeys($"{_context.TestUserProfile.Password}");
            _googleLoginPageModel.PasswordInput.SendKeys(Keys.Enter);
        }
    }
}
