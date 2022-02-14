using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class GoogleAuthenticatorAsyncPage : BasePage
    {
        public override string RelativePageUri => "Manage/EnrollGoogleAuthenticatorAsync";
        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@type=\"submit\"]"));
        public IWebElement PasscodeInput => _webDriver.FindElement(By.Id("passcodeInput"));
        public GoogleAuthenticatorAsyncPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }
    }
}
