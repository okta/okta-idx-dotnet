using embedded_sign_in_widget_e2etests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using System;

namespace embedded_sign_in_widget_e2etests.PageObjectModels
{
    public class GoogleLoginPage : BasePage
    {
        public override string RelativePageUri => throw new NotImplementedException();
        public IWebElement UserNameInput => TryFindElement(By.Id("identifierId"));
        public IWebElement PasswordInput => TryFindElement(By.XPath("//input[@type='password']"));
        
        public GoogleLoginPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration)
        { }

        public override void AssertPageOpenedAndValid()
        {
            _webDriver.Url.Should().StartWith("https://accounts.google.com/");
        }
    }
}
