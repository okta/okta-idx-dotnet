using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Helpers;
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
    public class SelectRecoveryAuthenticatorPageSteps : BasePageSteps
    {
        private SelectRecoveryAuthenticatorPage _selectRecoveryAuthenticatorPageModel;

        public SelectRecoveryAuthenticatorPageSteps(ITestConfig testConfig,
            ITestUserHelper userHelper,
            SelectRecoveryAuthenticatorPage selectRecoveryAuthenticatorPageModel)
            : base(testConfig)
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
