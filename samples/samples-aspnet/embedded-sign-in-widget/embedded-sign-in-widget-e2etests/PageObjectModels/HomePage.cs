using embedded_sign_in_widget_e2etests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.PageObjectModels
{
    public class HomePage : BasePage
    {
        public HomePage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }

        public IWebElement LoginButton => TryFindElement(By.Id("LoginBtn"));
        public IWebElement RegisterButton => TryFindElement(By.Id("RegisterBtn"));
        public IWebElement ClaimUserNameLabel => TryFindElement(By.Id("claim_preferred_username"));
        public IWebElement ClaimIdTokenLabel => TryFindElement(By.Id("claim_id_token"));
        public IWebElement ClaimAccessTokenLabel => TryFindElement(By.Id("claim_access_token"));
        public IWebElement ClaimRefreshTokenLabel => TryFindElement(By.Id("claim_refresh_token"));

        public override string RelativePageUri => string.Empty;

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
        }
    }
}
