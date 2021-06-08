using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class HomePageSteps: BaseTestSteps
    {
        private HomePage _homePageModel;
        private LoginPage _loginPageModel;
        private ResetPasswordPage _resetPasswordPageModel;
        private RegisterPage _registerPageModel;

        public HomePageSteps(ITestContext context, 
            HomePage homePageModel, 
            LoginPage loginPageModel,
            ResetPasswordPage resetPasswordPageModel,
            RegisterPage registerPageModel)
            :base(context)
        {
            _homePageModel = homePageModel;
            _loginPageModel = loginPageModel;
            _resetPasswordPageModel = resetPasswordPageModel;
            _registerPageModel = registerPageModel;
        }

        [Given(@"Mary navigates to the Basic Login View")]
        public void GivenMaryNavigatesToTheLoginPage()
        {
            _homePageModel.GoToPage();
            _homePageModel.AssertPageOpenedAndValid();
            _homePageModel.LoginButton.Click();
            _loginPageModel.AssertPageOpenedAndValid();
        }

        [Then(@"she is redirected to the Root View")]
        [Then(@"she is redirected to the Root Page is provided")]
        public void SheIsRedirectedToTheRootView()
        {
            _homePageModel.AssertPageOpenedAndValid();
        }

        [Given(@"Mary navigates to the Self Service Password Reset View")]
        public void GivenMaryNavigatesToTheSelfServicePasswordResetView()
        {
            _homePageModel.GoToPage();
            _homePageModel.LoginButton.Click();
            _loginPageModel.ForgotPasswordButton.Click();
            _resetPasswordPageModel.AssertPageOpenedAndValid();
        }

        [Given(@"Mary navigates to the Self Service Registration View")]
        public void GivenMaryNavigatesToTheSelfServiceRegistrationView()
        {
            _homePageModel.GoToPage();
            _homePageModel.AssertPageOpenedAndValid();
            _homePageModel.RegisterButton.Click();
            _registerPageModel.AssertPageOpenedAndValid();
        }

        [Then(@"the access_token is shown and not empty")]
        public void ThenTheAccess_TokenIsShownAndNotEmpty()
        {
            _homePageModel.ClaimAccessTokenLabel.Text.Should().NotBeEmpty();
        }

        [Then(@"the id_token is shown and not empty")]
        public void ThenTheId_TokenIsShownAndNotEmpty()
        {
            _homePageModel.ClaimIdTokenLabel.Text.Should().NotBeEmpty();
        }

        [Then(@"the preferred_username claim is shown and matches Mary's email")]
        public void ThenThePreferred_UsernameClaimIsShownAndMatchesMarySEmail()
        {
            _homePageModel.ClaimUserNameLabel.Text.Should().Be(_context.UserProfile.Email);
        }

        [Then(@"an application session is created")]
        public void ThenAnApplicationSessionIsCreated()
        {
            _homePageModel.ClaimAccessTokenLabel.Text.Should().NotBeEmpty();
            _homePageModel.ClaimIdTokenLabel.Text.Should().NotBeEmpty();
            _homePageModel.ClaimUserNameLabel.Text.Should().Be(_context.UserProfile.Email);
        }

    }
}
