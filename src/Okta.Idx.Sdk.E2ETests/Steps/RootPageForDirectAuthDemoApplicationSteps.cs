using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class RootPageForDirectAuthDemoApplicationSteps : BaseTestSteps
    {
        private HomePage _homePageModel;
        private LoginPage _loginPageModel;

        public RootPageForDirectAuthDemoApplicationSteps(ITestContext context,
            HomePage homePageModel,
            LoginPage loginPageModel)
            : base(context)
        {
            _homePageModel = homePageModel;
            _loginPageModel = loginPageModel;
        }

        [Given(@"Mary has an authenticated session")]
        public async Task GivenMaryHasAnAuthenticatedSession()
        {
            await _context.SetActivePasswordUserAsync("Mary");
            _loginPageModel.GoToPage();
            _loginPageModel.UserNameInput.SendKeys(_context.UserProfile.Email);
            _loginPageModel.PasswordInput.SendKeys(_context.UserProfile.Password);
            _loginPageModel.LoginButton.Click();
        }

        [Given(@"Mary navigates to the Root View")]
        public void GivenMaryNavigatesToTheRootView()
        {
            _homePageModel.GoToPage();
        }

        [When(@"Mary clicks the logout button")]
        public void WhenMaryClicksTheLogoutButton()
        {
            _homePageModel.LogoutButton.Click();
        }

        [Then(@"she is redirected back to the Root View")]
        public void ThenSheIsRedirectedBackToTheRootView()
        {
            _homePageModel.AssertPageOpenedAndValid();
        }

        [Then(@"Mary sees login, registration buttons")]
        public void ThenMarySeesLoginRegistrationButtons()
        {
            Action getLoginAndRegisterButtons = () =>
            {
                _ = _homePageModel.LoginButton;
                _ = _homePageModel.RegisterButton;
            };

            getLoginAndRegisterButtons.Should().NotThrow<NoSuchElementException>();
        }

        [Then(@"she does not see claims from /userinfo")]
        public void ThenSheDoesNotSeeClaimsFromUserinfo()
        {
            Func<IWebElement> getClaimAccessTokenLabel = () => _homePageModel.ClaimAccessTokenLabel;
            Func<IWebElement> getClaimIdTokenLabel = () => _homePageModel.ClaimIdTokenLabel;
            Func<IWebElement> getClaimUserNameTokenLabel = () => _homePageModel.ClaimUserNameLabel;

            getClaimAccessTokenLabel.Should().Throw<NoSuchElementException>();
            getClaimIdTokenLabel.Should().Throw<NoSuchElementException>();
            getClaimUserNameTokenLabel.Should().Throw<NoSuchElementException>();
        }
    }
}
