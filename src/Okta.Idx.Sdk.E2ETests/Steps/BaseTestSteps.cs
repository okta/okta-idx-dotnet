using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using Okta.Idx.Sdk.E2ETests.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    public abstract class BaseTestSteps
    {
        protected IWebDriver _webDriver;
 //       protected ITestConfig _configuration;
        protected UserProperties _testUser;
        protected ITestUserHelper _userHelper;
        private readonly IWebServerDriver _webServerDriver;

        public BaseTestSteps(WebDriverDriver webDriverDriver, /*ITestConfig configuration,*/ IWebServerDriver webServerDriver, ITestUserHelper userHelper)
        {
            _webDriver = webDriverDriver.WebDriver;
//            _configuration = configuration;
            _webServerDriver = webServerDriver;
            _userHelper = userHelper;
        }


        protected void WaitForTitleContains(string titleFragment, int timeoutSeconds)
        {
            var webDriverWait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(timeoutSeconds));
            webDriverWait.Until(_webDriver => _webDriver.Title.Contains(titleFragment));
        }

        protected void OpenRelativeUri(string uri)
        {
            _webDriver.Url = $"{_webServerDriver.SiteUrl}/{uri}";
        }

        protected void AssertTitleContains(string text)
        {
            _webDriver.Title.Should().Contain(text);
        }

        protected void ClickLinkWithText(string linkText, int timeoutSeconds = 0)
        {
            ElementByLinkText(linkText, timeoutSeconds).Click();
        }

        protected void ClickBtnById(string id, int timeoutSeconds = 0)
        {
            ElementById(id, timeoutSeconds).Click();
        }

        protected IWebElement ElementById(string id, int timeoutSeconds = 0)
        {
            return WaitForResult(driver => driver.FindElement(By.Id(id)), timeoutSeconds);
        }

        protected IWebElement ElementByLinkText(string linkText, int timeoutSeconds = 0)
        {
            return WaitForResult(driver=>driver.FindElement(By.LinkText(linkText)), timeoutSeconds);
        }

        private TResult WaitForResult<TResult>(Func<IWebDriver, TResult> func, int timeoutSeconds)
        {
            if (timeoutSeconds == 0)
            {
                return func(_webDriver);
            }
            var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(driver => func(driver));
        }
    }
}
