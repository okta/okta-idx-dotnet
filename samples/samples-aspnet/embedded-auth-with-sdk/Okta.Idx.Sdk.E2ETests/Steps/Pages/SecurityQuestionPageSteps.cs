using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using FluentAssertions;
using System;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class SecurityQuestionPageSteps : BaseTestSteps
    {
        private SecurityQuestionPage _securityQuestionPageModel;
        public SecurityQuestionPageSteps(ITestContext context, SecurityQuestionPage securityQuestionPageModel) : base(context)
        {
            _securityQuestionPageModel = securityQuestionPageModel;
        }


        [Then(@"she sees a screen to set up security question")]
        public void ThenSheSeesAScreenToSetupSecurityQuestion()
        {
            _securityQuestionPageModel.AssertPageOpenedAndValid();

        }

        [Then(@"she sees dropdown list of questions")]
        public void ThenSheSeesDropdownListOfQuestionsWithTheQuestionIsSelected()
        {
            _securityQuestionPageModel.QuestionDropDown.Displayed.Should().BeTrue();
        }

        [Then(@"she sees an input box to enter her answer")]
        public void ThenSheSeesAnInputBoxToEnterHerAnswer()
        {
            _securityQuestionPageModel.AnswerTextBox.Displayed.Should().BeTrue();
        }

        [When(@"she enters Okta")]
        public void WhenSheEnters()
        {
            _securityQuestionPageModel.AnswerTextBox.SendKeys("Okta");
        }

        [When(@"submits the form")]
        public void WhenSubmitsTheForm()
        {
            _securityQuestionPageModel.SubmitButton.Click();
        }
    }
}
