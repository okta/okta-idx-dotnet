using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class LoginPage : BasePage
    {
        public LoginPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }

        public override string RelativePageUri => "Account/Login";
        public IWebElement LoginButton => _webDriver.FindElement(By.Id("LoginBtn"));
        public IWebElement ForgotPasswordButton => _webDriver.FindElement(By.Id("ForgotPasswordBtn"));
        public IWebElement UserNameInput => _webDriver.FindElement(By.Id("UserName"));
        public IWebElement PasswordInput => _webDriver.FindElement(By.Id("Password"));

        public override void AssertPageOpenedAndValid() 
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Sign In");
        }
    }
}
