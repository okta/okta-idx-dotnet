using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    public class VerifyAuthenticatorPage : BasePage
    {
        public VerifyAuthenticatorPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration) { }

        public override string RelativePageUri => "Manage/VerifyAuthenticator";
        public IWebElement SubmitButton => _webDriver.FindElement(By.Id("submitBtn"));
        public IWebElement PasscodeInput => _webDriver.FindElement(By.Id("passcodeInput"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Verify your authenticator");
        }
    }
}
