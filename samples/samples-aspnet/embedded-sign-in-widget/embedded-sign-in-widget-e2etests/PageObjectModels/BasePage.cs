using embedded_sign_in_widget_e2etests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace embedded_sign_in_widget_e2etests.PageObjectModels
{
    public abstract class BasePage
    {
        protected IWebDriver _webDriver;
        protected string _baseUrl;

        public abstract string RelativePageUri { get; }
        public string Title => _webDriver.Title;
        public string FullPageUrl => $"{_baseUrl}/{RelativePageUri}";
        public bool IsPageOpened => _webDriver.Url.StartsWith(FullPageUrl);
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

        public Uri Uri
        {
            get => new Uri(_webDriver.Url);
        }

        public bool IsAtPath(string path)
        {
            return Uri.AbsolutePath.Equals(path);
        }

        protected IWebElement TryFindElement(By by)
        {
            int tryCount = 0;
            Exception thrown = null;
            int maxAttempts = 10;
            
            while (tryCount < maxAttempts)
            {
                try
                {
                    tryCount++;
                    var element = _webDriver.FindElement(by);
                    if (element.Displayed)
                    {
                        return element;
                    }
                }
                catch (Exception ex)
                {
                    thrown = ex;
                }
                Thread.Sleep(1000);
            }

            throw thrown ?? new Exception($"{by} was not found after {maxAttempts} attempts");
        }
    }

}
