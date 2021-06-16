using embedded_sign_in_widget_e2etests.Helpers;
using embedded_sign_in_widget_e2etests.PageObjectModels;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Steps.Pages
{
    [Binding]
    public class HomePageSteps : BaseTestSteps
    {
        private HomePage _homePageModel;
        private SignInWidgetPage _signInWidgetPageModel;

        public HomePageSteps(ITestContext context, HomePage homePageModel, SignInWidgetPage signInWidgetPageModel) : base(context)
        {
            _homePageModel = homePageModel;
            _signInWidgetPageModel = signInWidgetPageModel;
        }

        [Given(@"Mary navigates to the Embedded Widget View")]
        [Given(@"Mary navigates to Login with Social IDP")]
        public void GivenMaryNavigatesToTheEmbeddedWidgetView()
        {
            _homePageModel.GoToPage();
            _homePageModel.AssertPageOpenedAndValid();
            _homePageModel.LoginButton.Click();
            _signInWidgetPageModel.AssertPageOpenedAndValid();
        }

        [Then(@"she is redirected to the Root View")]
        [Then(@"she is redirected back to the Sample App")]

        public void ThenSheIsRedirectedToTheRootView()
        {
            _homePageModel.WaitForPageToOpen();
        }

        [Then(@"Mary is redirected back to the Root View")]
        public void ThenMaryIsRedirectedBackToTheRootView()
        {
            _homePageModel.ClaimAccessTokenLabel.Text.Should().NotBeEmpty();
            _homePageModel.ClaimIdTokenLabel.Text.Should().NotBeEmpty();
        }

        [Then(@"the access_token is shown and not empty")]
        public void ThenTheAccess_TokenIsShownAndNotEmpty()
        {
            _homePageModel.ClaimAccessTokenLabel.Text.Should().NotBeEmpty();
        }

        [Then(@"the id_token is shown and not empty")]
        public void ThenTheId_TokenIsShownAndNotEmpty()
        {
            _homePageModel.ClaimIdTokenLabel.Text.Should().NotBeEmpty();
        }

    }
}
