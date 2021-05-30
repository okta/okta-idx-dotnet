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
        protected TestUserProperties _testUser;
        protected ITestUserHelper _userHelper;

        public BaseTestSteps(WebDriverDriver webDriverDriver, ITestUserHelper userHelper)
        {
            _webDriver = webDriverDriver.WebDriver;
            _userHelper = userHelper;
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
