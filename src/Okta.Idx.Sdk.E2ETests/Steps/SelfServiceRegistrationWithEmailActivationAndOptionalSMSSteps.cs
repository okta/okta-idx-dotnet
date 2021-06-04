using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Helpers;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class SelfServiceRegistrationWithEmailActivationAndOptionalSMSSteps:BaseTestSteps
    {
        private HomePage _homePage;
        private RegisterPage _registerPage;
        private SelectAuthenticatorPage _selectAuthenticatorPage;
        private ChangePasswordPage _changePasswordPage;
        private VerifyAuthenticatorPage _verifyAuthenticatorPage;
        private EnrollPhoneAuthenticatorPage _enrollPhoneAuthenticatorPage;

        public SelfServiceRegistrationWithEmailActivationAndOptionalSMSSteps(ITestUserHelper userHelper, 
            HomePage homePage, 
            RegisterPage registerPage,
            SelectAuthenticatorPage selectAuthenticatorPage,
            ChangePasswordPage changePasswordPage,
            VerifyAuthenticatorPage verifyAuthenticatorPage,
            EnrollPhoneAuthenticatorPage enrollPhoneAuthenticatorPage)
            : base(userHelper)
        {
            _homePage = homePage;
            _registerPage = registerPage;
            _selectAuthenticatorPage = selectAuthenticatorPage;
            _changePasswordPage = changePasswordPage;
            _verifyAuthenticatorPage = verifyAuthenticatorPage;
            _enrollPhoneAuthenticatorPage = enrollPhoneAuthenticatorPage;
        }

        [Given(@"a Profile Enrollment policy defined assigning new users to the Everyone Group and by collecting ""(.*)"", ""(.*)"", and ""(.*)"", is allowed and assigned to a SPA, WEB APP or MOBILE application")]
        public void GivenAProfileEnrollmentPolicyDefinedAssigningNewUsersToTheEveryoneGroupAndByCollectingAndIsAllowedAndAssignedToASPAWEBAPPOrMOBILEApplication(string p0, string p1, string p2)
        { }
        
        [Given(@"""(.*)"" is selected for Email Verification under Profile Enrollment in Security > Profile Enrollment")]
        public void GivenIsSelectedForEmailVerificationUnderProfileEnrollmentInSecurityProfileEnrollment(string p0)
        { }
        
        [Given(@"configured Authenticators are Password \(required\), Email \(required\), and SMS \(optional\)")]
        public void GivenConfiguredAuthenticatorsArePasswordRequiredEmailRequiredAndSMSOptional()
        { }
        
        [Given(@"Mary does not have an account in the org")]
        public async Task GivenMaryDoesNotHaveAnAccountInTheOrg()
        {
            _testUser = await _userHelper.GetUnenrolledUser();
        }
        
        [Given(@"Mary navigates to the Self Service Registration View")]
        public void GivenMaryNavigatesToTheSelfServiceRegistrationView()
        {
            _homePage.GoToPage();
            _homePage.AssertPageOpenedAndValid();
            _homePage.RegisterButton.Click();
            _registerPage.AssertPageOpenedAndValid();
        }
        
        [When(@"she fills out her First Name")]
        public void WhenSheFillsOutHerFirstName()
        {
            _registerPage.FirstNameInput.SendKeys("Mary");
        }

        [When(@"she fills out her Last Name")]
        public void WhenSheFillsOutHerLastName()
        {
            _registerPage.LastNameInput.SendKeys("Lastname");
        }

        [When(@"she fills out her Email")]
        public void WhenSheFillsOutHerEmail()
        {
            _registerPage.EmailInput.SendKeys(_testUser.Email);
        }

        [When(@"she submits the registration form")]
        public void WhenSheSubmitsTheRegistrationForm()
        {
            _registerPage.SubmitButton.Click();
        }

        [Then(@"she sees the Select Authenticator page with password as the only option")]
        public void ThenSheSeesTheSelectAuthenticatorPageWithPasswordAsAnOnlyOption()
        {
            _selectAuthenticatorPage.AssertPageOpenedAndValid();
            Func<IWebElement> getPasswordFunc = () => _selectAuthenticatorPage.PasswordAuthenticator;
            getPasswordFunc.Should().NotThrow<NoSuchElementException>();
        }

        [When(@"she chooses password factor option")]
        public void WhenSheChoosesPasswordFactorOption()
        {
            _selectAuthenticatorPage.PasswordAuthenticator.Click();
        }

        [When(@"she submits the select authenticator form")]
        public void WhenSheSubmitsTheSelectAuthenticatorForm()
        {
            _selectAuthenticatorPage.SubmitButton.Click();
        }

        [Then(@"she sees the set new password form")]
        public void ThenSheSeesTheSetNewPasswordForm()
        {
            _changePasswordPage.AssertPageOpenedAndValid();
        }

        [When(@"she fills out her Password")]
        public void WhenSheFillsOutHerPassword()
        {
            _changePasswordPage.NewPasswordInput.SendKeys(_testUser.Password);
        }

        [When(@"she confirms her Password")]
        public void WhenSheConfirmsHerPassword()
        {
            _changePasswordPage.ConfirmPasswordInput.SendKeys(_testUser.Password);
        }

        [Then(@"she sees a list of factors to register")]
        [Then(@"she sees a list of required factors to setup")]
        public void ThenSheSeesAListOfRequiredFactorsToSetup()
        {
            _selectAuthenticatorPage.AssertPageOpenedAndValid();
        }

        [When(@"she selects Email")]
        public void WhenSheSelectsEmail()
        {
            _selectAuthenticatorPage.EmailAuthenticator.Click();
            _selectAuthenticatorPage.SubmitButton.Click();
        }

        [Then(@"she sees a page to input a code")]
        [Then(@"the screen changes to receive an input for a code")]
        public void ThenSheSeesAPageToInputACode()
        {
            _verifyAuthenticatorPage.AssertPageOpenedAndValid();
        }

        [When(@"she inputs the correct code from her email")]
        public async Task WhenSheInputsTheCorrectCodeFromHerEmail()
        {
            var theCode = await _userHelper.GetRecoveryCodeFromEmail();
            _verifyAuthenticatorPage.PasscodeInput.SendKeys(theCode);
            _verifyAuthenticatorPage.SubmitButton.Click();
        }

        [Then(@"she sees the list of optional factors \(SMS\)")]
        public void ThenSheSeesTheListOfOptionalFactorsSMS()
        {
            _selectAuthenticatorPage.AssertPageOpenedAndValid();
            Func<IWebElement> getSmsFactor = () => _selectAuthenticatorPage.PhoneAuthenticator;
            getSmsFactor.Should().NotThrow<NoSuchElementException>();
            _selectAuthenticatorPage.PhoneAuthenticator.Displayed.Should().BeTrue();
        }

        [When(@"she selects ""(.*)"" on SMS")]
        public void WhenSheSelectsOnSMS(string p0)
        {
            _selectAuthenticatorPage.SkipThisStepButton.Click();
        }
        
        [When(@"she selects Phone from the list")]
        public void WhenSheSelectsPhoneFromTheList()
        {
            _selectAuthenticatorPage.PhoneAuthenticator.Click();
            _selectAuthenticatorPage.SubmitButton.Click();
        }

        [When(@"She inputs a valid phone number")]
        public void WhenSheInputsAValidPhoneNumber()
        {
            _enrollPhoneAuthenticatorPage.PhoneNumberInput.SendKeys(_testUser.PhoneNumber);
            _enrollPhoneAuthenticatorPage.SubmitButton.Click();
        }
        
        [When(@"She selects ""(.*)""")]
        public void WhenSheSelects(string p0)
        {
        }
        
        [When(@"She inputs the correct code from her SMS")]
        public async Task WhenSheInputsTheCorrectCodeFromHerSMS()
        {
            var theCode = await _userHelper.GetRecoveryCodeFromSms();
            _verifyAuthenticatorPage.PasscodeInput.SendKeys(theCode);
            _verifyAuthenticatorPage.SubmitButton.Click();
        }

        [When(@"she fills out her Email with an invalid email format")]
        public void WhenSheFillsOutHerEmailWithAnInvalidEmailFormat()
        {
            _registerPage.EmailInput.SendKeys("invalid email format");
        }
        
        [When(@"she inputs an invalid phone number")]
        public void WhenSheInputsAnInvalidPhoneNumber()
        {
            _enrollPhoneAuthenticatorPage.PhoneNumberInput.SendKeys("+1");
        }
        
        [When(@"submits the enrollment form")]
        public void WhenSubmitsTheEnrollmentForm()
        {
            _enrollPhoneAuthenticatorPage.SubmitButton.Click();
        }
        
        [Then(@"an application session is created")]
        public void ThenAnApplicationSessionIsCreated()
        {
            _homePage.ClaimAccessTokenLabel.Text.Should().NotBeEmpty();
            _homePage.ClaimIdTokenLabel.Text.Should().NotBeEmpty();
            _homePage.ClaimUserNameLabel.Text.Should().Be(_testUser.Email);
        }
        
        [Then(@"she sees an error message ""(.*)""")]
        [Then(@"she should see an error message ""(.*)""")]
        public void ThenSheSeesAnErrorMessage(string errorMessage)
        {
            _registerPage.ValidationErrors.Text.Contains(errorMessage);
        }
        
    }
}
