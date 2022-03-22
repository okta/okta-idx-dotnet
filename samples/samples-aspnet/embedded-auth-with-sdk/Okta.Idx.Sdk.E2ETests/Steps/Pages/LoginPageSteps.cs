using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class LoginPageSteps : BaseTestSteps
    {
        private LoginPage _loginPageModel;

        public LoginPageSteps(ITestContext context,
            LoginPage loginPageModel)
            :base(context)
        {
            _loginPageModel = loginPageModel;
        }

        [When(@"she fills in her correct username")]
        [Given(@"she has inserted her username")]
        [When(@"she fills in her username")]
        public void WhenSheEntersCorrectUsername()
        {
            _loginPageModel.UserNameInput.SendKeys(_context.UserProfile.Email);
        }

        [When(@"she fills in her password")]
        [When(@"she fills in her correct password")]
        [Given(@"she has inserted her password")]
        public void WhenSheEntersCorrectPassword()
        {
            _loginPageModel.PasswordInput.SendKeys(_context.UserProfile.Password);
        }

        [Given(@"her password is correct")]
        public void GivenHerPasswordIsCorrect()
        {
        }

        [When(@"she submits the Login form")]
        [When(@"she clicks Login")]
        public void WhenSheSubmitsTheLoginForm()
        {
            _loginPageModel.LoginButton.Click();
        }

        [When(@"she fills in her incorrect password")]
        public void WhenSheFillsInHerIncorrectPassword()
        {
            _loginPageModel.PasswordInput.SendKeys("wrong password");
        }

        [When(@"she fills in her incorrect username")]
        public void WhenSheFillsInHerIncorrectUsername()
        {
            _loginPageModel.UserNameInput.SendKeys("wrongname@site.com");
        }

        [Then(@"she should see the message ""(.*)""")]
        public void ThenSheShouldSeeTheMessage(string authenticationError)
        {
            _loginPageModel.ValidationErrors.Text.Should().Contain(authenticationError);
        }

        [When(@"she clicks on the Forgot Password button")]
        public void WhenSheClicksOnThe()
        {
            _loginPageModel.ForgotPasswordButton.Click();
        }

        [When(@"she clicks the Login with Facebook button")]
        public void WhenSheClicksTheFacebookButton()
        {
            _loginPageModel.FacebookIdpButton.Click();
        }

        [When(@"she clicks the Login with Okta button")]
        public void WhenSheClicksTheOktaButton()
        {
            _loginPageModel.OktaIdpButton.Click();
        }

        [When(@"she sees a link to unlock her account")]
        public void WhenSheSeesALinkToUnlockHerAccount()
        {
        }

        [When(@"she clicks the link to unlock her account")]
        public void WhenSheClicksTheLinkToUnlockHerAccount()
        {
            _loginPageModel.UnlockAccountButton.Click();
        }

        [Then(@"she is redirected to the Basic Login View")]
        public void SheIsRedirectedToTheRootView()
        {
            _loginPageModel.AssertPageOpenedAndValid();
        }

        [Then(@"she should see the terminal message ""(.*)""")]
        public void SheShouldSeeTheMessage(string terminalMessage)
        {
            _loginPageModel.TerminalMessage.Text.Should().Contain(terminalMessage);
        }
    }
}
