using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Helpers;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class VerifyAuthenticatorPageSteps : BaseTestSteps
    {
        private VerifyAuthenticatorPage _verifyAuthenticatorPageModel;

        public VerifyAuthenticatorPageSteps(ITestContext context,
            VerifyAuthenticatorPage verifyAuthenticatorPageModel)
            : base(context)
        {
            _verifyAuthenticatorPageModel = verifyAuthenticatorPageModel;
        }

        [Then(@"she sees a page to input her code")]
        public void ThenSheSeesAPageToInputHerCode()
        {
            _verifyAuthenticatorPageModel.AssertPageOpenedAndValid();
        }

        [When(@"she fills in the correct code")]
        public async Task WhenSheFillsInTheCorrectCode()
        {
            var recoveryCode = await _context.GetRecoveryCodeFromEmail();
            recoveryCode.Should().NotBeNullOrEmpty();
            _verifyAuthenticatorPageModel.PasscodeInput.SendKeys(recoveryCode);
        }

        [When(@"She inputs the incorrect code from the email")]
        [When(@"She inputs the incorrect code from the sms")]
        public void WhenSheInputsTheIncorrectCodeFromTheEmail()
        {
            _verifyAuthenticatorPageModel.PasscodeInput.SendKeys("not a code");
            _verifyAuthenticatorPageModel.SubmitButton.Click();
        }

        [When(@"she submits the verification form")]
        public void WhenSheSubmitsTheVerificationForm()
        {
            _verifyAuthenticatorPageModel.SubmitButton.Click();
        }

        [Then(@"she sees a page to input a code")]
        [Then(@"the screen changes to receive an input for a code")]
        public void ThenSheSeesAPageToInputACode()
        {
            _verifyAuthenticatorPageModel.AssertPageOpenedAndValid();
        }

        [When(@"she inputs the correct code from her email")]
        [When(@"She inputs the correct code from the Email")]
        public async Task WhenSheInputsTheCorrectCodeFromHerEmail()
        {
            var theCode = await _context.GetRecoveryCodeFromEmail();
            _verifyAuthenticatorPageModel.PasscodeInput.SendKeys(theCode);
            _verifyAuthenticatorPageModel.SubmitButton.Click();
        }

        [When(@"She inputs the correct code from the SMS")]
        [When(@"She inputs the correct code from her SMS")]
        public async Task WhenSheInputsTheCorrectCodeFromHerSMS()
        {
            var theCode = await _context.GetRecoveryCodeFromSms();
            _verifyAuthenticatorPageModel.PasscodeInput.SendKeys(theCode);
            _verifyAuthenticatorPageModel.SubmitButton.Click();
        }

        [Then(@"the sample shows an error message ""(.*)"" on the Sample App")]
        [Then(@"the sample show as error message ""(.*)"" on the SMS Challenge page")]
        public void ThenTheSampleShowsAnErrorMessageOnTheSampleApp(string errorMessage)
        {
            _verifyAuthenticatorPageModel.ValidationErrors.Text.Should().Contain(errorMessage);
        }


        [Then(@"she sees a field to re-enter another code")]
        public void ThenSheSeesAFieldToRe_EnterAnotherCode()
        {
            _verifyAuthenticatorPageModel.PasscodeInput.Displayed.Should().BeTrue();
        }
    }
}
