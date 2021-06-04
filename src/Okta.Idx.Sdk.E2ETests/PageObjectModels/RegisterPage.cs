using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    [Binding]
    public class RegisterPage : BasePage
    {
        public override string RelativePageUri => "Account/Register";
        public RegisterPage(WebDriverDriver webDriverDriver, ITestConfig testConfiguration)
            : base(webDriverDriver, testConfiguration)
        { }

        public IWebElement FirstNameInput => _webDriver.FindElement(By.Id("FirstName"));
        public IWebElement LastNameInput => _webDriver.FindElement(By.Id("LastName"));
        public IWebElement EmailInput => _webDriver.FindElement(By.Id("Email"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@type=\"submit\"]"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Register");
        }

    }
}
