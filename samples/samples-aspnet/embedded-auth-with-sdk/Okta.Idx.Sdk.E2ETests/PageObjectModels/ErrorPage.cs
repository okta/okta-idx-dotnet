using embedded_auth_with_sdk.E2ETests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    [Binding]
    public class ErrorPage : BasePage
    {
        public override string RelativePageUri => throw new NotImplementedException();
        public IWebElement ErrorText => _webDriver.FindElement(By.XPath("//div[@class=\"container body-content\"]"));

        public ErrorPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }
        public override void AssertPageOpenedAndValid()
        {
            Title.Should().StartWith("Error -");
        }
    }
}
