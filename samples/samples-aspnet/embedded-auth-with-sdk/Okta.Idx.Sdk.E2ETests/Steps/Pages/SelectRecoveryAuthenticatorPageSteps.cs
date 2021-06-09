using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class SelectRecoveryAuthenticatorPageSteps : BaseTestSteps
    {
        private SelectRecoveryAuthenticatorPage _selectRecoveryAuthenticatorPageModel;

        public SelectRecoveryAuthenticatorPageSteps(ITestContext context,
            SelectRecoveryAuthenticatorPage selectRecoveryAuthenticatorPageModel)
            : base(context)
        {
            _selectRecoveryAuthenticatorPageModel = selectRecoveryAuthenticatorPageModel;
        }

        [Then(@"she sees a page to select an authenticator")]
        public void ThenSheSeesAPageToSelectAnAuthenticator()
        {
            _selectRecoveryAuthenticatorPageModel.AssertPageOpenedAndValid();
        }

        [When(@"she chooses Email")]
        public void ThenSheChoosesEmail()
        {
            var emailOption = _selectRecoveryAuthenticatorPageModel.FindAuthenticatorOption("Email");
            emailOption.Should().NotBeNull();
            emailOption.Click();
        }

        [When(@"she submits the select form")]
        public void ThenSheSubmitsTheForm()
        {
            _selectRecoveryAuthenticatorPageModel.SubmitButton.Click();
        }

    }
}
