using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class ResetPasswordPage : BasePage
    {
        public ResetPasswordPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }
        public override string RelativePageUri => "Account/ForgotPassword";
        public IWebElement UserNameInput => _webDriver.FindElement(By.Id("UserName"));
        public IWebElement ResetPasswordButton => _webDriver.FindElement(By.Id("ResetPasswordBtn"));
        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Enter your email to recover your password.");
        }
    }
}
