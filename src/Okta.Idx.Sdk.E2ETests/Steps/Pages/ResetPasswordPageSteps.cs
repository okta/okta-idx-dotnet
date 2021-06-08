using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class ResetPasswordPageSteps : BasePageSteps
    {
        private ResetPasswordPage _resetPasswordPageModel;

        private const string WrongEmailAddress = "wrongemail@gmail.kp";

        public ResetPasswordPageSteps(ITestConfig testConfig,
            ResetPasswordPage resetPasswordPageModel)
            : base(testConfig)
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
            _resetPasswordPageModel.UserNameInput.SendKeys(_testConfig.TestUser.Email);
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
