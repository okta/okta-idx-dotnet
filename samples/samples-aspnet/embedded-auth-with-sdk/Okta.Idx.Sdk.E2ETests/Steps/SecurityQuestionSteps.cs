using System;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps
{
    [Binding]
    public class SecurityQuestionSteps : BaseTestSteps
    {
        public SecurityQuestionSteps(ITestContext context) : base(context)
        {
        }

        [When(@"she selects Security Question")]
        public void WhenSheSelectsSecurityQuestion()
        {
            throw new PendingStepException();
        }

        [Then(@"she sees a screen to ""([^""]*)""")]
        public void ThenSheSeesAScreenTo(string p0)
        {
            throw new PendingStepException();
        }

        [Then(@"she sees dropdown list of questions with the question ""([^""]*)"" is selected")]
        public void ThenSheSeesDropdownListOfQuestionsWithTheQuestionIsSelected(string p0)
        {
            throw new PendingStepException();
        }

        [Then(@"she sees an input box to enter her answer")]
        public void ThenSheSeesAnInputBoxToEnterHerAnswer()
        {
            throw new PendingStepException();
        }

        [When(@"she enters ""([^""]*)""")]
        public void WhenSheEnters(string okta)
        {
            throw new PendingStepException();
        }

        [When(@"submits the form")]
        public void WhenSubmitsTheForm()
        {
            throw new PendingStepException();
        }
    }
}
