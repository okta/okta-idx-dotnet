using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using Okta.Idx.Sdk.E2ETests.Helpers;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class BasicLoginWithPasswordFactorSteps : BaseTestSteps
    {
        private HomePage _homePage;
        private LoginPage _loginPage;
        private ResetPasswordPage _resetPasswordPage;

        public BasicLoginWithPasswordFactorSteps(WebDriverDriver webDriverDriver, 
                                                ITestUserHelper userHelper, 
                                                HomePage homePage, 
                                                LoginPage loginPage,
                                                ResetPasswordPage resetPasswordPage)
            : base(webDriverDriver, userHelper)
        {
            _homePage = homePage;
            _loginPage = loginPage;
            _resetPasswordPage = resetPasswordPage;
        }

        [Given(@"a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required")]
        public void GivenASPAWEBAPPOrMOBILESignOnPolicyThatDefinesPasswordAsRequired()
        { }

        [Given(@"the list of Authenticators contains Email and Password")]
        public void GivenTheListOfAuthenticatorsContainsEmailAndPassword()
        { }

        [Given(@"a User named ""(.*)"" exists, and this user has already setup email and password factors")]
        public async Task GivenAUserNamedExistsAndThisUserHasAlreadySetupEmailAndPasswordFactors(string p0)
        {
            _testUser = await _userHelper.GetActivePasswordUserAsync();
        }

        [Given(@"Mary navigates to the Basic Login View")]
        public void GivenMaryNavigatesToTheLoginPage()
        {
            _homePage.GoToPage();
            _homePage.AssertPageOpenedAndValid();
            _homePage.LoginButton.Click();
            _loginPage.AssertPageOpenedAndValid();
        }

        [When(@"she fills in her correct username")]
        public void WhenSheEntersCorrectUsername()
        {
            _loginPage.UserNameInput.SendKeys(_testUser.Email);
        }

        [When(@"she fills in her password")]
        [When(@"she fills in her correct password")]
        public void WhenSheEntersCorrectPassword()
        {
            _loginPage.PasswordInput.SendKeys(_testUser.Password);
        }

        [When(@"she submits the Login form")]
        public void WhenSheSubmitsTheLoginForm()
        {
            _loginPage.LoginButton.Click();
        }

        [Then(@"she is redirected to the Root View")]
        public void SheIsRedirectedToTheRootView()
        {
            _homePage.AssertPageOpenedAndValid();
        }

        [Then(@"the access_token is shown and not empty")]
        public void ThenTheAccess_TokenIsShownAndNotEmpty()
        {
            _homePage.ClaimAccessTokenLabel.Text.Should().NotBeEmpty();
        }

        [Then(@"the id_token is shown and not empty")]
        public void ThenTheId_TokenIsShownAndNotEmpty()
        {
            _homePage.ClaimIdTokenLabel.Text.Should().NotBeEmpty();
        }

        [Then(@"the preferred_username claim is shown and matches Mary's email")]
        public void ThenThePreferred_UsernameClaimIsShownAndMatchesMarySEmail()
        {
            _homePage.ClaimUserNameLabel.Text.Should().Be(_testUser.Email);
        }

        [When(@"she fills in her incorrect password")]
        public void WhenSheFillsInHerIncorrectPassword()
        {
            _loginPage.PasswordInput.SendKeys("wrong password");
        }

        [When(@"she fills in her incorrect username")]
        public void WhenSheFillsInHerIncorrectUsername()
        {
            _loginPage.UserNameInput.SendKeys("wrongname@site.com");
        }

        [Then(@"she should see a message on the Login form ""(.*)""")]
        public void ThenSheShouldSeeAMessageOnTheLoginForm(string p0)
        {
            _loginPage.ValidationErrorText.Text.Should().Contain("There is no account with the Username");
        }

        [Then(@"she should see the message ""(.*)""")]
        public void ThenSheShouldSeeTheMessage(string authenticationError)
        {
            _loginPage.ValidationErrorText.Text.Should().Contain(authenticationError);
        }

        [When(@"she clicks on the Forgot Password button")]
        public void WhenSheClicksOnThe()
        {
            _loginPage.ForgotPasswordButton.Click();
        }

        [Then(@"she is redirected to the Self Service Password Reset View")]
        public void ThenSheIsRedirectedToTheSelfServicePasswordResetView()
        {
            _resetPasswordPage.AssertPageOpenedAndValid();
        }

    }
}
