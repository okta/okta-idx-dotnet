using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    public class SelectAuthenticatorAsyncPage : BasePage
    {
        public override string RelativePageUri => "Manage/SelectAuthenticatorAsync";
        public SelectAuthenticatorAsyncPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration)
        { }

        public IWebElement SmsAuthenticator => _webDriver.FindElement(By.XPath("//label[contains(normalize-space(.),\"sms\")]"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@Value=\"Submit\"]"));
    }
}
