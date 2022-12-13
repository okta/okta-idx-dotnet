using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class SelectAuthenticatorPageSteps : BaseTestSteps
    {
        private SelectAuthenticatorPage _selectAuthenticatorPageModel;
        private SelectAuthenticatorAsyncPage _selectAuthenticatorAsyncPageModel;

        public SelectAuthenticatorPageSteps(ITestContext context,
            SelectAuthenticatorPage selectAuthenticatorPageModel,
            SelectAuthenticatorAsyncPage selectAuthenticatorAsyncPageModel)
            : base(context)
        {
            _selectAuthenticatorPageModel = selectAuthenticatorPageModel;
            _selectAuthenticatorAsyncPageModel = selectAuthenticatorAsyncPageModel;
        }

        [Then(@"She sees a list of factors")]
        [Then(@"she sees a list of factors to register")]
        [Then(@"she sees a list of required factors to setup")]
        [Then(@"she is presented with a list of factors")]
        public void ThenSheIsPresentedWithAnOptionToSelectEmailToVerify()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
        }

        [When(@"she selects Email")]
        [When(@"She selects Email from the list")]
        [When(@"She has selected Email from the list of factors")]
        public void WhenSheSelectsEmail()
        {
            _selectAuthenticatorPageModel.EmailAuthenticator.Click();
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

        [Then(@"she sees the list of optional factors \(SMS\)")]
        [Then(@"she is presented with an option to select Phone")]
        [Then(@"she is presented with an option to select SMS to enroll")]
        public void ThenSheSeesTheListOfOptionalFactorsSMS()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
            Func<IWebElement> getSmsFactor = () => _selectAuthenticatorPageModel.PhoneAuthenticator;
            getSmsFactor.Should().NotThrow<NoSuchElementException>();
            _selectAuthenticatorPageModel.PhoneAuthenticator.Displayed.Should().BeTrue();
        }

        [Then(@"she sees the list of optional factors")]
        public void ThenSheSeesTheListOfOptionalFactors()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
        }


        [Then(@"she is presented with an option to select SMS to verify")]
        public void ThenSheIsPresentedWithAnOptionToSelectSmsToVerify()
        {
            _selectAuthenticatorAsyncPageModel.AssertPageOpenedAndValid();
            Func<IWebElement> getSmsFactor = () => _selectAuthenticatorAsyncPageModel.SmsAuthenticator;
            getSmsFactor.Should().NotThrow<NoSuchElementException>();
            _selectAuthenticatorAsyncPageModel.SmsAuthenticator.Displayed.Should().BeTrue();
        }

        [Then(@"she is presented with an option to select Email to verify")]
        public void SheIsPresentedWithAnOptionToSelectEmailToVerify()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
            Func<IWebElement> getSmsFactor = () => _selectAuthenticatorPageModel.EmailAuthenticator;
            getSmsFactor.Should().NotThrow<NoSuchElementException>();
            _selectAuthenticatorPageModel.EmailAuthenticator.Displayed.Should().BeTrue();
        }

        [When(@"She selects SMS from the list")]
        public void WhenSheSelectsSMSFromTheList()
        {
            _selectAuthenticatorAsyncPageModel.SmsAuthenticator.Click();
            _selectAuthenticatorAsyncPageModel.SubmitButton.Click();
        }

        [When(@"she selects Phone")]
        [When(@"She selects Phone from the list")]
        [When(@"she selects Phone from the list")]
        public void WhenSheSelectsPhone()
        {            
            _selectAuthenticatorPageModel.PhoneAuthenticator.Click();
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

        [Then(@"she sees the Select Authenticator page with password as the only option")]
        [Then(@"she sees the Select Authenticator page")]
        public void ThenSheSeesTheSelectAuthenticatorPageWithPasswordAsAnOnlyOption()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
            Func<IWebElement> getPasswordFunc = () => _selectAuthenticatorPageModel.PasswordAuthenticator;
            getPasswordFunc.Should().NotThrow<NoSuchElementException>();
        }

        [When(@"she chooses password factor option")]
        public void WhenSheChoosesPasswordFactorOption()
        {
            _selectAuthenticatorPageModel.PasswordAuthenticator.Click();
        }

        [When(@"she selects Security Question")]
        public void WhenSheSelectsSecurityQuestion()
        {
            _selectAuthenticatorPageModel.SecurityQuestion.Click();
            _selectAuthenticatorAsyncPageModel.SubmitButton.Click();
        }

        [When(@"she submits the select authenticator form")]
        public void WhenSheSubmitsTheSelectAuthenticatorForm()
        {
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

        [When(@"she selects Skip")]
        public void WhenSheSelectsOnSMS()
        {
            _selectAuthenticatorPageModel.SkipThisStepButton.Click();
        }


        [When(@"she skips optional authenticators if prompted")]
        public void WhenSheSkipsOptionalAuthenticatorsIfPrompted()
        {
            if (_selectAuthenticatorPageModel.IsPageOpened)
            {
                _selectAuthenticatorPageModel.SkipThisStepButton.Click();
            }
        }


        [Then(@"she sees the list of required factors \(Google Authenticator\) to enroll")]

        [Then(@"she is presented with an option to select Google Authenticator to verify")]
        public void ThenSheSeesTheListOfRequiredFactorsGoogleAuthenticatorToEnroll()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
            Func<IWebElement> getPasswordFunc = () => _selectAuthenticatorPageModel.GoogleAuthenticator;
            getPasswordFunc.Should().NotThrow<NoSuchElementException>();
        }

        [When(@"She selects Google Authenticator from the list")]
        public void WhenSheSelectsGoogleAuthenticatorFromTheList()
        {
            _selectAuthenticatorPageModel.GoogleAuthenticator.Click();
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

    }
}
