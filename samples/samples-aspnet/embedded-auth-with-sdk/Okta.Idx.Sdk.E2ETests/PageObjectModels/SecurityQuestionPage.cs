using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public class SecurityQuestionPage : BasePage
    {
        public SecurityQuestionPage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration) : base(webDriverDriver, testConfiguration)
        {
        }

        public override string RelativePageUri => "SecurityQuestion/EnrollSecurityQuestion";

        public IWebElement QuestionDropDown => TryFindElement(By.Id("QuestionKey"));
        
        public IWebElement AnswerTextBox => TryFindElement(By.Id("Answer"));

        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@type=\"submit\"]"));
    }
}
