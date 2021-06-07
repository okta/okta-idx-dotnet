using Okta.Idx.Sdk.E2ETests.PageObjectModels;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class RegisterPageSteps : BaseTestSteps
    {
        private RegisterPage _registerPageModel;

        public RegisterPageSteps(ITestContext context,
            RegisterPage registerPageModel)
            : base(context)
        {
            _registerPageModel = registerPageModel;
        }

        [When(@"she fills out her First Name")]
        public void WhenSheFillsOutHerFirstName()
        {
            _registerPageModel.FirstNameInput.SendKeys(_context.UserProfile.FirstName);
        }

        [When(@"she fills out her Last Name")]
        public void WhenSheFillsOutHerLastName()
        {
            _registerPageModel.LastNameInput.SendKeys(_context.UserProfile.LastName);
        }

        [When(@"she fills out her Email")]
        public void WhenSheFillsOutHerEmail()
        {
            _registerPageModel.EmailInput.SendKeys(_context.UserProfile.Email);
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
