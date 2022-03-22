using System;
using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps
{
    [Binding]
    public class UnlockAccountWithTOTPSteps : BaseTestSteps
    {
        private UnlockAccountPage _unlockAccountPageModel;
        public UnlockAccountWithTOTPSteps(ITestContext context, UnlockAccountPage unlockAccountPageModel) : base(context)
        {
            _unlockAccountPageModel = unlockAccountPageModel;
        }

        [Given(@"Password Policy is set to Lock out user after (.*) unsuccessful attempt")]
        public void GivenPasswordPolicyIsSetToLockOutUserAfterUnsuccessfulAttempt(int p0)
        {
        }
        
        [Given(@"the Password Policy Rule ""(.*)"" has ""(.*)"" checked")]
        public void GivenThePasswordPolicyRuleHasChecked(string p0, string p1)
        {
        }
        
        //[Given(@"the Password Policy Rule ""(.*)"" has ""(.*)"" and ""(.*)"" checked")]
        //public void GivenThePasswordPolicyRuleHasAndChecked(string p0, string p1, string p2)
        //{
        //}

        [When(@"she inputs her email")]
        public void WhenSheInputsHerEmail()
        {
            _unlockAccountPageModel.UserNameInput.SendKeys(_context.UserProfile.Email);
        }

        [When(@"she selects Email to unlock her account")]
        public void WhenSheSelectsEmail()
        {
            _unlockAccountPageModel.EmailAuthenticator.Click();
            _unlockAccountPageModel.SubmitButton.Click();
        }


        [Then(@"she sees a page to input her user name and select Email, Phone, or Okta Verify to unlock her account")]
        public void ThenSheSeesAPageToInputHerUserNameAndSelectEmailPhoneOrOktaVerifyToUnlockHerAccount()
        {
            _unlockAccountPageModel.AssertPageOpenedAndValid();
        }


        //[When(@"she enters the OTP from her email in the original tab")]
        //public void WhenSheEntersTheOTPFromHerEmailInTheOriginalTab()
        //{
        //    ScenarioContext.Current.Pending();
        //}

        //[When(@"submits the form")]
        //public void WhenSubmitsTheForm()
        //{
        //    ScenarioContext.Current.Pending();
        //}

        //[Then(@"she sees a page to input her user name and select Email, Phone, or Okta Verify to unlock her account")]
        //public void ThenSheSeesAPageToInputHerUserNameAndSelectEmailPhoneOrOktaVerifyToUnlockHerAccount()
        //{
        //    ScenarioContext.Current.Pending();
        //}

        //[Then(@"she should see a screen telling her to ""(.*)""")]
        //public void ThenSheShouldSeeAScreenTellingHerTo(string p0)
        //{
        //    ScenarioContext.Current.Pending();
        //}

        //[Then(@"she should see an input box for a code to enter from the email")]
        //public void ThenSheShouldSeeAnInputBoxForACodeToEnterFromTheEmail()
        //{
        //    ScenarioContext.Current.Pending();
        //}

        //[Then(@"she should see a terminal page that says ""(.*)""")]
        //public void ThenSheShouldSeeATerminalPageThatSays(string p0)
        //{
        //    ScenarioContext.Current.Pending();
        //}

        //[Then(@"she should see a link on the page to go back to the Basic Login View")]
        //public void ThenSheShouldSeeALinkOnThePageToGoBackToTheBasicLoginView()
        //{
        //    ScenarioContext.Current.Pending();
        //}
    }
}
