using embedded_sign_in_widget_e2etests.Helpers;
using embedded_sign_in_widget_e2etests.PageObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Steps.Pages
{
    [Binding]
    public class SignInWidgetPageSteps : BaseTestSteps
    {
        private SignInWidgetPage _signInWidgetPage;

        public SignInWidgetPageSteps(ITestContext context, SignInWidgetPage signInPageModel) : base(context)
        {
            _signInWidgetPage = signInPageModel;
        }

        [When(@"she fills in her correct username")]
        public void WhenSheFillsInHerCorrectUsername()
        {
            _signInWidgetPage.UserNameInput.SendKeys(_context.TestUserProfile.Email);
        }

        [When(@"she fills in her correct password")]
        public void WhenSheFillsInHerCorrectPassword()
        {
            _signInWidgetPage.PasswordInput.SendKeys(_context.TestUserProfile.Password);
        }

        [When(@"she submits the Login form")]
        public void WhenSheSubmitsTheLoginForm()
        {
            _signInWidgetPage.NextButton.Click();
        }

        [When(@"she clicks the Login with Google button in the embedded Sign In Widget")]
        public void WhenSheClicksTheButtonInTheEmbeddedSignInWidget()
        {
            _signInWidgetPage.SignonWithGoogleButton.Click();
        }

        [When(@"she clicks the Login with Facebook button")]
        public void WhenSheClicksTheFacebookButton()
        {
            _signInWidgetPage.SignonWithFacebookButton.Click();
        }
    }
}
