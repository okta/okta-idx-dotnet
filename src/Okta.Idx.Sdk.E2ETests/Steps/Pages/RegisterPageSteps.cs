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
    public class RegisterPageSteps : BasePageSteps
    {
        private RegisterPage _registerPageModel;

        public RegisterPageSteps(ITestConfig testConfig,
            RegisterPage registerPageModel)
            : base(testConfig)
        {
            _registerPageModel = registerPageModel;
        }

        [When(@"she fills out her First Name")]
        public void WhenSheFillsOutHerFirstName()
        {
            _registerPageModel.FirstNameInput.SendKeys(_testConfig.TestUser.FirstName);
        }

        [When(@"she fills out her Last Name")]
        public void WhenSheFillsOutHerLastName()
        {
            _registerPageModel.LastNameInput.SendKeys(_testConfig.TestUser.LastName);
        }

        [When(@"she fills out her Email")]
        public void WhenSheFillsOutHerEmail()
        {
            _registerPageModel.EmailInput.SendKeys(_testConfig.TestUser.Email);
        }

        [When(@"she submits the registration form")]
        public void WhenSheSubmitsTheRegistrationForm()
        {
            _registerPageModel.SubmitButton.Click();
        }

        [When(@"she fills out her Email with an invalid email format")]
        public void WhenSheFillsOutHerEmailWithAnInvalidEmailFormat()
        {
            _registerPageModel.EmailInput.SendKeys("invalid email format");
        }

        [Then(@"she sees an error message ""(.*)""")]
        [Then(@"she should see an error message ""(.*)""")]
        public void ThenSheSeesAnErrorMessage(string errorMessage)
        {
            _registerPageModel.ValidationErrors.Text.Contains(errorMessage);
        }

    }
}
