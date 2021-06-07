using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
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
    }
}
