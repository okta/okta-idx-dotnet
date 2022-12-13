using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class ResetPasswordPageSteps : BaseTestSteps
    {
        private ResetPasswordPage _resetPasswordPageModel;

        private const string WrongEmailAddress = "wrongemail@gmail.kp";

        public ResetPasswordPageSteps(ITestContext context,
            ResetPasswordPage resetPasswordPageModel)
            : base(context)
        {
            _resetPasswordPageModel = resetPasswordPageModel;
        }

        [Then(@"she is redirected to the Self Service Password Reset View")]
        public void ThenSheIsRedirectedToTheSelfServicePasswordResetView()
        {
            _resetPasswordPageModel.AssertPageOpenedAndValid();
        }

        [When(@"she inputs her correct Email")]
        public void WhenSheInputsHerCorrectEmail()
        {
            _resetPasswordPageModel.UserNameInput.SendKeys(_context.UserProfile.Email);
        }

        [When(@"she submits the recovery form")]
        public void WhenSheSubmitsTheRecoveryForm()
        {
            _resetPasswordPageModel.ResetPasswordButton.Click();
        }

        [When(@"she inputs an Email that doesn't exist")]
        public void WhenSheInputsAnEmailThatDoesnTExist()
        {
            _resetPasswordPageModel.UserNameInput.SendKeys(WrongEmailAddress);
        }

        [Then(@"she sees the Password Recovery Page")]
        public void ThenSheSeesThePasswordRecoveryPage()
        {
            _resetPasswordPageModel.AssertPageOpenedAndValid();
        }

        [Then(@"she sees a message ""(.*)""")]
        public void ThenSheSeesAMessage(string partialErrorMessage)
        {
            var expectedErrorMessage = partialErrorMessage.Replace("{username}", WrongEmailAddress);
            Func<IWebElement> getErrorsFunc = () => _resetPasswordPageModel.ValidationErrors;
            getErrorsFunc.Should().NotThrow<NoSuchElementException>();
            getErrorsFunc().Text.Should().Contain(expectedErrorMessage);
        }
    }
}
