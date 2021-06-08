using embedded_sign_in_widget_e2etests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.PageObjectModels
{
    public class HomePage : BasePage
    {
        public HomePage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }

        public IWebElement LoginButton => _webDriver.FindElement(By.Id("LoginBtn"));
        public IWebElement RegisterButton => _webDriver.FindElement(By.Id("RegisterBtn"));
        public IWebElement ClaimUserNameLabel => _webDriver.FindElement(By.Id("claim-preferred_username"));
        public IWebElement ClaimIdTokenLabel => _webDriver.FindElement(By.Id("claim-id_token"));
        public IWebElement ClaimAccessTokenLabel => _webDriver.FindElement(By.Id("claim-access_token"));
        public IWebElement ClaimRefreshTokenLabel => _webDriver.FindElement(By.Id("claim-refresh_token"));

        public override string RelativePageUri => string.Empty;

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Home Page");
        }
    }
}
