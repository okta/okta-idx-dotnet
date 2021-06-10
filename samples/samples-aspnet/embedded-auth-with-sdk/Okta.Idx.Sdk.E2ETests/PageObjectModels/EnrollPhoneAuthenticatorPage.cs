using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class EnrollPhoneAuthenticatorPage : BasePage
    {
        public EnrollPhoneAuthenticatorPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration) { }

        public override string RelativePageUri => "Manage/EnrollPhoneAuthenticator";
        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@type=\"submit\"]"));
        public IWebElement PhoneNumberInput => _webDriver.FindElement(By.Id("PhoneNumber"));
        public IWebElement SmsRadioinput => _webDriver.FindElement(By.XPath("//input[@type=\"radio\"][@value=\"sms\"]"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Setup phone authentication");
        }
    }
}
