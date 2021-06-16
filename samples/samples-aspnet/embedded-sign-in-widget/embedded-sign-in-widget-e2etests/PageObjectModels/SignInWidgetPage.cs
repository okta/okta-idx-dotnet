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
        public IWebElement UserNameInput => TryFindElement(By.Id("input3"));
        public IWebElement PasswordInput => TryFindElement(By.Id("input5"));
        public IWebElement NextButton => TryFindElement(By.XPath("//*[@id=\"form1\"]/div[2]/input"));
        public IWebElement SignonWithGoogleButton => TryFindElement(By.XPath("//a[@data-se=\"social-auth-google-button\"]"));
    }
}
