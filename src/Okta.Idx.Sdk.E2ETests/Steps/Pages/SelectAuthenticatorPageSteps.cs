using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Helpers;
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
    public class SelectAuthenticatorPageSteps : BasePageSteps
    {
        private ITestUserHelper _userHelper;
        private SelectAuthenticatorPage _selectAuthenticatorPageModel;

        public SelectAuthenticatorPageSteps(ITestConfig testConfig,
            ITestUserHelper userHelper,
            SelectAuthenticatorPage selectAuthenticatorPageModel)
            : base(testConfig)
        {
            _userHelper = userHelper;
            _selectAuthenticatorPageModel = selectAuthenticatorPageModel;
        }


        [Then(@"She sees a list of factors")]
        [Then(@"she sees a list of factors to register")]
        [Then(@"she sees a list of required factors to setup")]
        [Then(@"she is presented with an option to select Email to verify")]
        public void ThenSheIsPresentedWithAnOptionToSelectEmailToVerify()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
        }

        [When(@"she selects Email")]
        [When(@"She has selected Email from the list of factors")]
        public void WhenSheSelectsEmail()
        {
            _selectAuthenticatorPageModel.EmailAuthenticator.Click();
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

        [When(@"She selects Email from the list")]
        public void WhenSheSelectsEmailFromTheList()
        {
            _selectAuthenticatorPageModel.EmailAuthenticator.Click();
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

        [Then(@"she sees the Select Authenticator page with password as the only option")]
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

        [When(@"she submits the select authenticator form")]
        public void WhenSheSubmitsTheSelectAuthenticatorForm()
        {
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

        [Then(@"she sees the list of optional factors \(SMS\)")]
        public void ThenSheSeesTheListOfOptionalFactorsSMS()
        {
            _selectAuthenticatorPageModel.AssertPageOpenedAndValid();
            Func<IWebElement> getSmsFactor = () => _selectAuthenticatorPageModel.PhoneAuthenticator;
            getSmsFactor.Should().NotThrow<NoSuchElementException>();
            _selectAuthenticatorPageModel.PhoneAuthenticator.Displayed.Should().BeTrue();
        }

        [When(@"she selects ""(.*)"" on SMS")]
        public void WhenSheSelectsOnSMS(string p0)
        {
            _selectAuthenticatorPageModel.SkipThisStepButton.Click();
        }

        [When(@"she selects Phone from the list")]
        public void WhenSheSelectsPhoneFromTheList()
        {
            _selectAuthenticatorPageModel.PhoneAuthenticator.Click();
            _selectAuthenticatorPageModel.SubmitButton.Click();
        }

    }
}
