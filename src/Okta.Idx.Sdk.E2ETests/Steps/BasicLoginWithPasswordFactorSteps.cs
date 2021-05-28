using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using Okta.Idx.Sdk.E2ETests.Helpers;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class BasicLoginWithPasswordFactorSteps : BaseTestSteps
    {
        public BasicLoginWithPasswordFactorSteps(WebDriverDriver webDriverDriver, IWebServerDriver webServerDriver, ITestUserHelper userHelper) 
            : base(webDriverDriver, webServerDriver, userHelper)
        {
        }

        [Given(@"a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required")]
        public void GivenASPAWEBAPPOrMOBILESignOnPolicyThatDefinesPasswordAsRequired()
        {
            
        }

        [Given(@"the list of Authenticators contains Email and Password")]
        public void GivenTheListOfAuthenticatorsContainsEmailAndPassword()
        {
            
        }

        [Given(@"a User named ""(.*)"" exists, and this user has already setup email and password factors")]
        public void GivenAUserNamedExistsAndThisUserHasAlreadySetupEmailAndPasswordFactors(string p0)
        {
            _testUser = _userHelper.GetActivePasswordUser();
        }

        [Given(@"Mary navigates to the Basic Login View")]
        public void GivenMaryNavigatesToTheLoginPage()
        {
            OpenRelativeUri("/");
            AssertTitleContains("Home Page");
            ClickLinkWithText("Log in", 10);
            AssertTitleContains("Login");
        }

        [When(@"she fills in her correct username")]
        public void WhenSheEntersCorrectUsername()
        {
            ElementById("UserName").SendKeys(_testUser.Email);
        }

        [When(@"she fills in her password")]
        [When(@"she fills in her correct password")]
        public void WhenSheEntersCorrectPassword()
        {
            ElementById("Password").SendKeys(_testUser.Password);
        }

        [When(@"she submits the Login form")]
        public void WhenSheSubmitsTheLoginForm()
        {
            ElementById("LogInBtn").Click();
        }

        [Then(@"she is redirected to the Root View")]
        public void SheIsRedirectedToTheRootView()
        {
            AssertTitleContains("Home Page");
        }

        [Then(@"Mary should get logged-in")]
        public void ThenMaryShouldGetLogged_In()
        {
            AssertTitleContains("Home Page");
            ElementByLinkText($"Hello, {_testUser.Email}!").Displayed.Should().BeTrue();
        }

        [When(@"she fills in her incorrect password")]
        public void WhenSheFillsInHerIncorrectPassword()
        {
            ElementById("Password").SendKeys("wrong password");
        }

        [When(@"she fills in her incorrect username")]
        public void WhenSheFillsInHerIncorrectUsername()
        {
            ElementById("UserName").SendKeys("wrongname@site.com");
        }        

        [Then(@"she should see a message on the Login form ""(.*)""")]
        public void ThenSheShouldSeeAMessageOnTheLoginForm(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [When(@"she submits the Login form  with blank fields")]
        public void WhenSheSubmitsTheLoginFormWithBlankFields()
        {
//            ScenarioContext.Current.Pending();
        }

        [Then(@"she should see the message ""(.*)""")]
        public void ThenSheShouldSeeTheMessage(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [Given(@"Mary is not a member of the ""(.*)"" group")]
        public void GivenMaryIsNotAMemberOfTheGroup(string p0)
        {
            _testUser = _userHelper.GetUnassignedUser();
        }

        [Then(@"she sees the login form again with blank fields")]
        public void ThenSheSeesTheLoginFormAgainWithBlankFields()
        {
//            ScenarioContext.Current.Pending();
        }

        [Then(@"should see the message ""(.*)""")]
        public void ThenShouldSeeTheMessage(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [When(@"she clicks on the ""(.*)""")]
        public void WhenSheClicksOnThe(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [Then(@"she is redirected to the Self Service Password Reset View")]
        public void ThenSheIsRedirectedToTheSelfServicePasswordResetView()
        {
//            ScenarioContext.Current.Pending();
        }

    }
}
