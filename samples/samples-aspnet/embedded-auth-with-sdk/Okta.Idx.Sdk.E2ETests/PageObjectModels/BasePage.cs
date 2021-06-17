using FluentAssertions;
using embedded_auth_with_sdk.E2ETests.Drivers;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace embedded_auth_with_sdk.E2ETests.PageObjectModels
{
    public abstract class BasePage
    {
        protected IWebDriver _webDriver;
        protected string _baseUrl;

        public abstract string RelativePageUri { get; }
        public string Title => _webDriver.Title;
        public string FullPageUrl => $"{_baseUrl}/{RelativePageUri}";
        public bool IsPageOpened => WaitForCondition(()=>_webDriver.Url.StartsWith(FullPageUrl));
        public IWebElement ValidationErrors => _webDriver.FindElement(By.XPath("//div[@class=\"validation-summary-errors text-danger\"]"));
        public BasePage(WebDriverDriver webDriverDriver, ITestConfiguration testConfiguration)
        {
            _webDriver = webDriverDriver.WebDriver;
            _baseUrl = testConfiguration.SiteUrl;
        }

        public virtual void AssertPageOpenedAndValid()
        {
            IsPageOpened.Should().BeTrue();
        }

        public void GoToPage()
        {
            _webDriver.Url = FullPageUrl;
        }

        protected IWebElement TryFindElement(By by)
        {
            int tryCount = 0;
            Exception thrown;
            while (true)
            {
                try
                {
                    return _webDriver.FindElement(by);
                }
                catch (Exception ex)
                {
                    thrown = ex;
                }

                if (++tryCount >= 10)
                {
                    break;
                }

                Thread.Sleep(200);
            }

            throw new Exception($"Cannot find the element with this condition: {by}\n{thrown.Message}");
        }

        protected bool WaitForCondition (Func<bool> condition)
        {
            int tryCount = 0;
            while (tryCount < 10)
            {
                    tryCount++;
                    if (condition())
                        return true;
                    Thread.Sleep(1000);
            }
            return false;
        }
    }
}
