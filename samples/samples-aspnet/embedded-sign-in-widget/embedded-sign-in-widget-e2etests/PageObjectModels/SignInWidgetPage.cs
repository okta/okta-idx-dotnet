using embedded_sign_in_widget_e2etests.Drivers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.PageObjectModels
{
    public class SignInWidgetPage : BasePage
    {
        public SignInWidgetPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        { }

        public override string RelativePageUri => "Account/SignInWidget";
        public IWebElement UserNameInput => TryFindElement(By.CssSelector("[name='identifier']"));
        public IWebElement PasswordInput => TryFindElement(By.CssSelector("[name='credentials.passcode']"));
        public IWebElement NextButton => TryFindElement(By.CssSelector("[type='submit']"));
        public IWebElement SignonWithOktaIdpButton => TryFindElement(By.XPath("//a[@data-se=\"social-auth-general-idp-button\"]"));
        public IWebElement SelectEmailButton => TryFindElement(By.CssSelector("[data-se='okta_email']"));
    }
}
