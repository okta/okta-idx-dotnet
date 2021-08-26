using System;
using TechTalk.SpecFlow;
using embedded_sign_in_widget_e2etests.Helpers;
using embedded_sign_in_widget_e2etests.PageObjectModels;

namespace embedded_sign_in_widget_e2etests.Steps
{
    [Binding]
    public class SignInWithPasswordSteps : BaseTestSteps
    {
        private SignInWidgetPage _signInWidgetPage;

        public SignInWithPasswordSteps(ITestContext context, SignInWidgetPage signInWidgetPage) : base(context)
        {
            _signInWidgetPage = signInWidgetPage;
        }

        [Given(@"a SPA, WEB APP or MOBILE Sign On Policy that defines Password as required")]
        public void GivenASPAWEBAPPOrMOBILESignOnPolicyThatDefinesPasswordAsRequired()
        {
            // no action necessary
        }
        
        [Given(@"the list of Authenticators contains Email and Password")]
        public void GivenTheListOfAuthenticatorsContainsEmailAndPassword()
        {
            // no action necessary
        }

        [Given(@"a User named Mary exists, and this user has already setup email and password factors")]
        public void GivenAUserNamedMaryExistsAndThisUserHasAlreadySetupEmailAndPasswordFactors()
        {
            // no action necessary
        }


        [When(@"she fills in her correct username")]
        public void WhenSheFillsInHerCorrectUsername()
        {
            _signInWidgetPage.UserNameInput.SendKeys(_context.Configuration.OktaUserEmail);
        }

        [When(@"she fills in her correct password")]
        public void WhenSheFillsInHerCorrectPassword()
        {
            _signInWidgetPage.PasswordInput.SendKeys(_context.Configuration.OktaUserPassword);
        }

        [When(@"she submits the Login form")]
        public void WhenSheSubmitsTheLoginForm()
        {
            _signInWidgetPage.NextButton.Click();
        }
    }
}
