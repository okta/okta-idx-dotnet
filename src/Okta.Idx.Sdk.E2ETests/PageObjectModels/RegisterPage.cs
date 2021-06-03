using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    [Binding]
    public class RegisterPage : BasePage
    {
        public override string RelativePageUri => "Account/Register";
        public RegisterPage(WebDriverDriver webDriverDriver, ITestConfig testConfiguration)
            : base(webDriverDriver, testConfiguration)
        { }

        public IWebElement FirstNameInput => _webDriver.FindElement(By.Id("FirstName"));
        public IWebElement LastNameInput => _webDriver.FindElement(By.Id("LastName"));
        public IWebElement EmailInput => _webDriver.FindElement(By.Id("Email"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.XPath("//input[@type=\"submit\"]"));

        public override void AssertPageOpenedAndValid()
        {
            base.AssertPageOpenedAndValid();
            Title.Should().StartWith("Register");
        }

        //[When(@"she fills out her Last Name")]
        //public void WhenSheFillsOutHerLastName()
        //{
        //    LastNameInput.SendKeys("Lastname");
        //}

        //[When(@"she fills out her Email")]
        //public void WhenSheFillsOutHerEmail()
        //{
        //  //  EmailInput.SendKeys(_testUser.Email);
        //}

        //[When(@"she submits the registration form")]
        //public void WhenSheSubmitsTheRegistrationForm()
        //{
        //    SubmitButton.Click();
        //}

    }
}
