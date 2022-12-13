using embedded_auth_with_sdk.E2ETests.PageObjectModels;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps.Pages
{
    [Binding]
    public class ErrorPageSteps : BaseTestSteps
    {
        private ErrorPage _errorPageModel;

        public ErrorPageSteps(ITestContext context, ErrorPage errorPageModel) : base(context)
        {
            _errorPageModel = errorPageModel;
        }

        [Then(@"Mary should see an error message ""(.*)""")]
        public void ThenMaryShouldSeeAnErrorMessage(string expectedError)
        {
            _errorPageModel.ErrorText.Text.Should().Contain(expectedError);
        }
    }
}
