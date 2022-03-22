using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using embedded_auth_with_sdk.E2ETests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    [Binding]
    public class UnlockAccountPage : BasePage
    {
        public override string RelativePageUri => "Account/UnlockAccountAsync";
        
        public UnlockAccountPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration)
        { }

        public IWebElement UserNameInput => _webDriver.FindElement(By.Id("UserName"));
        public IWebElement EmailAuthenticator => _webDriver.FindElement(By.XPath("//label[contains(normalize-space(.),\"Email\")]"));
        public IWebElement PhoneAuthenticator => _webDriver.FindElement(By.XPath("//label[contains(normalize-space(.),\"Phone\")]"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@type=\"submit\"]"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Unlock");
        }
    }
}
