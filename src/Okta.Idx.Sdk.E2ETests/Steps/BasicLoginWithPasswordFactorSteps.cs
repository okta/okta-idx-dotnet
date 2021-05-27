﻿using A18NAdapter;
using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class BasicLoginWithPasswordFactorSteps : BaseTestSteps
    {
        public BasicLoginWithPasswordFactorSteps(WebDriverDriver webDriver, ITestConfig configuration, IWebServerDriver webServerDriver, A18nAdapter a18nAdapter) 
            : base(webDriver, configuration, webServerDriver, a18nAdapter)
        {
        }

        [Given(@"Mary navigates to the Basic Login View")]
        public void GivenMaryNavigatesToTheLoginPage()
        {
            OpenRelativeUri("/");
            AssertTitleContains("Home Page");
            ClickLinkWithText("Log in", 10);
            AssertTitleContains("Login");
        }
        
        [When(@"she enters correct credentials")]
        public void WhenSheEntersCorrectCredentials()
        {
            ElementById("UserName").SendKeys(_testUser.Email);
            ElementById("Password").SendKeys(_testUser.Password);
        }

        [When(@"she submits the Login form")]
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

        [When(@"she fills in her incorrect username with password")]
        public void WhenSheFillsInHerIncorrectUsernameWithPassword()
        {
            ElementById("UserName").SendKeys("wrong_name@mail.com");
            ElementById("Password").SendKeys(_testUser.Password);
        }

        [When(@"she fills in her correct username and incorrect password")]
        public void WhenSheFillsInHerCorrectUsernameAndIncorrectPassword()
        {
            ElementById("UserName").SendKeys(_testUser.Email);
            ElementById("Password").SendKeys("password");
        }

        [Then(@"she should see a message on the Login form ""(.*)""")]
        public void ThenSheShouldSeeAMessageOnTheLoginForm(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [When(@"she submits the Login form  with blank fields")]
        public void WhenSheSubmitsTheLoginFormWithBlankFields()
        {
//            ScenarioContext.Current.Pending();
        }

        [Then(@"she should see the message ""(.*)""")]
        public void ThenSheShouldSeeTheMessage(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [Given(@"Mary is not a member of the ""(.*)"" group")]
        public void GivenMaryIsNotAMemberOfTheGroup(string p0)
        {
            _testUser.Email = _configuration.UnassignedUser;
        }

        [Then(@"she sees the login form again with blank fields")]
        public void ThenSheSeesTheLoginFormAgainWithBlankFields()
        {
//            ScenarioContext.Current.Pending();
        }

        [Then(@"should see the message ""(.*)""")]
        public void ThenShouldSeeTheMessage(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [When(@"she clicks on the ""(.*)""")]
        public void WhenSheClicksOnThe(string p0)
        {
//            ScenarioContext.Current.Pending();
        }

        [Then(@"she is redirected to the Self Service Password Reset View")]
        public void ThenSheIsRedirectedToTheSelfServicePasswordResetView()
        {
//            ScenarioContext.Current.Pending();
        }

    }
}
