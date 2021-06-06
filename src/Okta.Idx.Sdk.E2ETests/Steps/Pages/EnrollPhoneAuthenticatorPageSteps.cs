using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class EnrollPhoneAuthenticatorPageSteps : BasePageSteps
    {
        private EnrollPhoneAuthenticatorPage _enrollPhoneAuthenticatorPageModel;

        public EnrollPhoneAuthenticatorPageSteps(ITestConfig testConfig,
            EnrollPhoneAuthenticatorPage enrollPhoneAuthenticatorPageModel)
            : base(testConfig)
        {
            _enrollPhoneAuthenticatorPageModel = enrollPhoneAuthenticatorPageModel;
        }

        [When(@"She inputs a valid phone number")]
        public void WhenSheInputsAValidPhoneNumber()
        {
            _enrollPhoneAuthenticatorPageModel.PhoneNumberInput.SendKeys(_testConfig.TestUser.PhoneNumber);
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

    }
}
