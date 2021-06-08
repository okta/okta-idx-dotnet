using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class EnrollPhoneAuthenticatorPageSteps : BaseTestSteps
    {
        private EnrollPhoneAuthenticatorPage _enrollPhoneAuthenticatorPageModel;

        public EnrollPhoneAuthenticatorPageSteps(ITestContext context,
            EnrollPhoneAuthenticatorPage enrollPhoneAuthenticatorPageModel)
            : base(context)
        {
            _enrollPhoneAuthenticatorPageModel = enrollPhoneAuthenticatorPageModel;
        }

        [When(@"She inputs a valid phone number")]
        public void WhenSheInputsAValidPhoneNumber()
        {
            _enrollPhoneAuthenticatorPageModel.PhoneNumberInput.SendKeys(_context.UserProfile.PhoneNumber);
            _enrollPhoneAuthenticatorPageModel.SubmitButton.Click();
        }

        [When(@"She selects ""(.*)""")]
        public void WhenSheSelects(string p0)
        {
        }

        [When(@"she inputs an invalid phone number")]
        public void WhenSheInputsAnInvalidPhoneNumber()
        {
            _enrollPhoneAuthenticatorPageModel.PhoneNumberInput.SendKeys("+1");
        }

        [When(@"submits the enrollment form")]
        public void WhenSubmitsTheEnrollmentForm()
        {
            _enrollPhoneAuthenticatorPageModel.SubmitButton.Click();
        }

        [Then(@"she should see a message ""(.*)""")]
        public void ThenSheShouldSeeAMessage(string errorMessage)
        {
            _enrollPhoneAuthenticatorPageModel.ValidationErrors.Text.Should().Contain(errorMessage);
        }

    }
}
