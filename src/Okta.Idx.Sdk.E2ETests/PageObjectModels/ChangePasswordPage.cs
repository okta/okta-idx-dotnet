using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    public class ChangePasswordPage:BasePage
    {
        public ChangePasswordPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }

        public override string RelativePageUri => "Manage/ChangePassword";
        public IWebElement NewPasswordInput => _webDriver.FindElement(By.Id("NewPassword"));
        public IWebElement ConfirmPasswordInput => _webDriver.FindElement(By.Id("ConfirmPassword"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.Id("SubmitBtn"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().Contain("password");
        }
    }
}
