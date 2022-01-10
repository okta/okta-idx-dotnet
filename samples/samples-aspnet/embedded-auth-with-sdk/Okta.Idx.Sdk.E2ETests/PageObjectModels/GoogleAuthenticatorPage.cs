using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class GoogleAuthenticatorPage : BasePage
    {
        public override string RelativePageUri => "Manage/EnrollGoogleAuthenticator";

        public IWebElement NextButton => _webDriver.FindElement(By.XPath("//input[@Value=\"Next\"]"));
        public IWebElement SharedSecretInput => _webDriver.FindElement(By.Id("SharedSecret"));
        public IWebElement QrCodeImage => _webDriver.FindElement(By.Id("qrCodeImg"));

        public GoogleAuthenticatorPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }
    }
}
