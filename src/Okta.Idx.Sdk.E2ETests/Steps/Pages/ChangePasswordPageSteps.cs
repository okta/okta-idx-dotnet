using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using System;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class ChangePasswordPageSteps : BaseTestSteps
    {
        private ChangePasswordPage _changePasswordPageModel;

        private const string NewPasswordStarts = "OneC0mpl3xP@S5w0rd!";
        private string AllNewComplexPassword = string.Empty;

        public ChangePasswordPageSteps(ITestContext context,
            ChangePasswordPage changePasswordPageModel)
            : base(context)
        {
            _changePasswordPageModel = changePasswordPageModel;
        }

        [Then(@"she sees the set new password form")]
        [Then(@"she sees a page to set her password")]
        public void ThenSheSeesAPageToSetHerPassword()
        {
            _changePasswordPageModel.AssertPageOpenedAndValid();
        }

        [When(@"she fills out her Password")]
        public void WhenSheFillsOutHerPassword()
        {
            _changePasswordPageModel.NewPasswordInput.SendKeys(_context.UserProfile.Password);
        }

        [When(@"she confirms her Password")]
        public void WhenSheConfirmsHerPassword()
        {
            _changePasswordPageModel.ConfirmPasswordInput.SendKeys(_context.UserProfile.Password);
        }

        [When(@"she fills a password that fits within the password policy")]
        public void WhenSheFillsAPasswordThatFitsWithinThePasswordPolicy()
        {
            AllNewComplexPassword = $"{NewPasswordStarts}-{DateTime.UtcNow}";
            _changePasswordPageModel.NewPasswordInput.SendKeys(AllNewComplexPassword);
        }

        [When(@"she confirms that password")]
        public void WhenSheConfirmsThatPassword()
        {
            _changePasswordPageModel.ConfirmPasswordInput.SendKeys(AllNewComplexPassword);
        }

        [When(@"she submits the change password form")]
        public void WhenSheSubmitsTheChangePasswordForm()
        {
            _changePasswordPageModel.SubmitButton.Click();
        }
    }
}
