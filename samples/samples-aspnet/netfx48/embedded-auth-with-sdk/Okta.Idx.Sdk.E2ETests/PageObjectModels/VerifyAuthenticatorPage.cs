using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class VerifyAuthenticatorPage : BasePage
    {
        public VerifyAuthenticatorPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration) { }

        public override string RelativePageUri => "Manage/VerifyAuthenticator";
        public IWebElement ResendCodeButton => _webDriver.FindElement(By.Id("resendCodeBtn"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.Id("submitBtn"));
        public IWebElement PasscodeInput => _webDriver.FindElement(By.Id("passcodeInput"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Verify your authenticator");
        }
    }
}
