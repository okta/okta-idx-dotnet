using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class BasicLoginWithPasswordFactorSteps : BaseTestSteps
    {
        public BasicLoginWithPasswordFactorSteps(WebDriverDriver webDriver, ITestConfig configuration, IWebServerDriver webServerDriver) 
            : base(webDriver, configuration, webServerDriver)
        {
        }

        [Given(@"Mary navigates to the login page")]
        public void GivenMaryNavigatesToTheLoginPage()
        {
            OpenRelativeUri("/");
            AssertTitleContains("Home Page");
            ClickLinkWithText("Log in", 10);
            AssertTitleContains("Login");
        }
        
        [When(@"she enters valid credentials")]
        public void WhenSheEntersValidCredentials()
        {
            ElementById("UserName").SendKeys(_configuration.NormalUser);
            ElementById("Password").SendKeys(_configuration.UserPassword);
        }

        [When(@"she submits the login form")]
        public void WhenSheSubmitsTheLoginForm()
        {
            ElementById("LogInBtn").Click();
        }

        [Then(@"Mary should get logged-in")]
        public void ThenMaryShouldGetLogged_In()
        {
            AssertTitleContains("Home Page");
            ElementByLinkText($"Hello, {_configuration.NormalUser}!").Displayed.Should().BeTrue();
        }        
    }
}
