using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class SelectUnlockAccountAuthenticatorAsyncPage : BasePage
    {
        public override string RelativePageUri => "Manage/SelectUnlockAccountAuthenticatorAsync";
        public SelectUnlockAccountAuthenticatorAsyncPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration)
        { }

        public IWebElement SmsAuthenticator => _webDriver.FindElement(By.XPath("//label[contains(normalize-space(.),\"sms\")]"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@Value=\"Submit\"]"));
        
        public IWebElement PhoneAuthenticator => _webDriver.FindElement(By.XPath("//label[contains(normalize-space(.),\"Phone\")]"));
    }
}
