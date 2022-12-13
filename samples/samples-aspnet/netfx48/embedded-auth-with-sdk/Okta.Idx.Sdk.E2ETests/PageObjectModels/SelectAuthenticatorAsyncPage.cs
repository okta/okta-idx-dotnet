using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
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
