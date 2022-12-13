using embedded_auth_with_sdk.E2ETests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class OktaOidcIdpLoginPage : BasePage
    {
        public override string RelativePageUri => throw new NotImplementedException();
        public IWebElement UserNameInput => TryFindElement(By.XPath("//input[@name=\"identifier\"]"));
        public IWebElement PasswordInput => TryFindElement(By.XPath("//input[@type=\"password\"]"));
        public IWebElement LoginButton => TryFindElement(By.XPath("//input[@type=\"submit\"]"));

        public OktaOidcIdpLoginPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
            : base(webDriverDriver, testConfiguration)
        { }

        public override void AssertPageOpenedAndValid()
        {
            Title.Should().StartWith("Sign In with Okta Idp");
            _webDriver.Url.Should().Contain("okta.com");
        }
    }
}
