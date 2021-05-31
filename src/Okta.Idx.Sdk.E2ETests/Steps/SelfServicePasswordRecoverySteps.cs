using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using Okta.Idx.Sdk.E2ETests.Helpers;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class SelfServicePasswordRecoverySteps : BaseTestSteps
    {
        private HomePage _homePage;
        private LoginPage _loginPage;
        private ResetPasswordPage _resetPasswordPage;
        private SelectRecoveryAuthenticatorPage _selectAuthenticatorPage;
        private VerifyAuthenticatorPage _verifyAuthenticatorPage;

        public SelfServicePasswordRecoverySteps(WebDriverDriver webDriverDriver,
                                                ITestUserHelper userHelper,
                                                HomePage homePage,
                                                LoginPage loginPage,
                                                ResetPasswordPage resetPasswordPage,
                                                SelectRecoveryAuthenticatorPage selectAuthenticatorPage,
                                                VerifyAuthenticatorPage verifyAuthenticatorPage)
            : base(webDriverDriver, userHelper)
        {
            _homePage = homePage;
            _loginPage = loginPage;
            _resetPasswordPage = resetPasswordPage;
            _selectAuthenticatorPage = selectAuthenticatorPage;
            _verifyAuthenticatorPage = verifyAuthenticatorPage;
        }

        [Given(@"an org with an ORG Policy that defines Authenticators with Password and Email as required")]
        public void GivenAnOrgWithAnORGPolicyThatDefinesAuthenticatorsWithPasswordAndEmailAsRequired()
        { }

        [Given(@"a user named ""(.*)""")]
        public void GivenAUserNamed(string p0)
        { }

        [Given(@"Mary is a user with a verified email and a set password")]
        public async Task GivenMaryIsAUserWithAVerifiedEmailAndASetPasswordAsync()
        {
            _testUser = await _userHelper.GetActivePasswordUserAsync();
        }

        [Given(@"Mary navigates to the Self Service Password Reset View")]
        public void GivenMaryNavigatesToTheSelfServicePasswordResetView()
        {
            _homePage.GoToPage();
            _homePage.LoginButton.Click();
            _loginPage.ForgotPasswordButton.Click();
            _resetPasswordPage.AssertPageOpenedAndValid();
        }

        [When(@"she inputs her correct Email")]
        public void WhenSheInputsHerCorrectEmail()
        {
            _resetPasswordPage.UserNameInput.SendKeys(_testUser.Email);
        }

        [When(@"she submits the recovery form")]
        public void WhenSheSubmitsTheRecoveryForm()
        {
            _resetPasswordPage.ResetPasswordButton.Click();
        }

        [Then(@"she sees a page to select an authenticator")]
        public void ThenSheSeesAPageToSelectAnAuthenticator()
        {
            _selectAuthenticatorPage.AssertPageOpenedAndValid();
        }

        [When(@"she chooses Email")]
        public void ThenSheChoosesEmail()
        {
            var emailOption = _selectAuthenticatorPage.FindAuthenticatorOption("Email");
            emailOption.Should().NotBeNull();
            emailOption.Click();
        }

        [When(@"she submits the select form")]
        public void ThenSheSubmitsTheForm()
        {
            _selectAuthenticatorPage.SubmitButton.Click();
        }

        [Then(@"she sees a page to input her code")]
        public void ThenSheSeesAPageToInputHerCode()
        {
            _verifyAuthenticatorPage.AssertPageOpenedAndValid();
        }

        [When(@"she fills in the correct code")]
        public async Task WhenSheFillsInTheCorrectCode()
        {
            var recoveryCode = await _userHelper.GetRecoveryCodeFromEmail();
            recoveryCode.Should().NotBeNullOrEmpty();
            _verifyAuthenticatorPage.PasscodeInput.SendKeys(recoveryCode);
        }

        [When(@"she submits the form")]
        public void WhenSheSubmitsTheForm()
        {
            _verifyAuthenticatorPage.SubmitButton.Click();
        }

        [When(@"she fills a password that fits within the password policy")]
        public void WhenSheFillsAPasswordThatFitsWithinThePasswordPolicy()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"she confirms that password")]
        public void WhenSheConfirmsThatPassword()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"she selects ""(.*)""")]
        public void WhenSheSelects(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"she inputs an Email that doesn't exist")]
        public void WhenSheInputsAnEmailThatDoesnTExist()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she sees a page to set her password")]
        public void ThenSheSeesAPageToSetHerPassword()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she is redirected to the Root Page is provided")]
        public void ThenSheIsRedirectedToTheRootPageIsProvided()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she sees the Password Recovery Page")]
        public void ThenSheSeesThePasswordRecoveryPage()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she sees a message ""(.*)""")]
        public void ThenSheSeesAMessage(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
