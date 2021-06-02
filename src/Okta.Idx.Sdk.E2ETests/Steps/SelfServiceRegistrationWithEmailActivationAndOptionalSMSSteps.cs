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

        public SelfServiceRegistrationWithEmailActivationAndOptionalSMSSteps(ITestUserHelper userHelper, 
            HomePage homePage, 
            RegisterPage registerPage,
            SelectAuthenticatorPage selectAuthenticatorPage) 
            : base(userHelper)
        {
            _homePage = homePage;
            _registerPage = registerPage;
            _selectAuthenticatorPage = selectAuthenticatorPage;
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
            _homePage.AssertPageOpenedAndValid();
            _homePage.RegisterButton.Click();
            _registerPage.AssertPageOpenedAndValid();
        }
        
        [When(@"she fills out her First Name")]
        public void WhenSheFillsOutHerFirstName()
        {
            _registerPage.FirstNameInput.SendKeys("User");
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

        [Then(@"she sees the Select Authenticator page with password as an only opton")]
        public void ThenSheSeesTheSelectAuthenticatorPageWithPasswordAsAnOnlyOpton()
        {
            _selectAuthenticatorPage.AssertPageOpenedAndValid();
            Func<IWebElement> getPasswordFunc = () => _selectAuthenticatorPage.PasswordAuthenticator;
            getPasswordFunc.Should().NotThrow<NoSuchElementException>();
        }

        [When(@"she choses password factor option")]
        public void WhenSheChosesPasswordFactorOption()
        {
            _selectAuthenticatorPage.PasswordAuthenticator.Click();
        }

        [When(@"she submits the enroll password form")]
        public void WhenSheSubmitsTheEnrollPasswordForm()
        {
            _selectAuthenticatorPage.SubmitButton.Click();
        }

        [Then(@"she sees the set new password form")]
        public void ThenSheSeesTheSetNewPasswordForm()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she fills out her Password")]
        public void ThenSheFillsOutHerPassword()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she confirms her Password")]
        public void ThenSheConfirmsHerPassword()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"she submits the change password form")]
        public void ThenSheSubmitsTheChangePasswordForm()
        {
            ScenarioContext.Current.Pending();
        }


        [When(@"she fills out her Password")]
        public void WhenSheFillsOutHerPassword()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"she confirms her Password")]
        public void WhenSheConfirmsHerPassword()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"she selects Email")]
        public void WhenSheSelectsEmail()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"she inputs the correct code from her email")]
        public void WhenSheInputsTheCorrectCodeFromHerEmail()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"she selects ""(.*)"" on SMS")]
        public void WhenSheSelectsOnSMS(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"she selects Phone from the list")]
        public void WhenSheSelectsPhoneFromTheList()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"She inputs a valid phone number")]
        public void WhenSheInputsAValidPhoneNumber()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"She selects ""(.*)""")]
        public void WhenSheSelects(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"She inputs the correct code from her SMS")]
        public void WhenSheInputsTheCorrectCodeFromHerSMS()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"she fills out her Email with an invalid email format")]
        public void WhenSheFillsOutHerEmailWithAnInvalidEmailFormat()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"she inputs an invalid phone number")]
        public void WhenSheInputsAnInvalidPhoneNumber()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"submits the enrollment form")]
        public void WhenSubmitsTheEnrollmentForm()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"she sees a list of required factors to setup")]
        public void ThenSheSeesAListOfRequiredFactorsToSetup()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"she sees a page to input a code")]
        public void ThenSheSeesAPageToInputACode()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"she sees the list of optional factors \(SMS\)")]
        public void ThenSheSeesTheListOfOptionalFactorsSMS()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"an application session is created")]
        public void ThenAnApplicationSessionIsCreated()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"she sees a list of available factors to setup")]
        public void ThenSheSeesAListOfAvailableFactorsToSetup()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"she sees a list of factors to register")]
        public void ThenSheSeesAListOfFactorsToRegister()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the screen changes to receive an input for a code")]
        public void ThenTheScreenChangesToReceiveAnInputForACode()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"she sees an error message ""(.*)""")]
        public void ThenSheSeesAnErrorMessage(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"she should see an error message ""(.*)""")]
        public void ThenSheShouldSeeAnErrorMessage(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
