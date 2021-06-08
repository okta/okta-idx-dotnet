using FluentAssertions;
using Okta.Idx.Sdk.E2ETests.Drivers;
using OpenQA.Selenium;

namespace Okta.Idx.Sdk.E2ETests.PageObjectModels
{
    public abstract class BasePage
    {
        protected IWebDriver _webDriver;
        protected string _baseUrl;

        public abstract string RelativePageUri { get; }
        public string Title => _webDriver.Title;
        public string FullPageUrl => $"{_baseUrl}/{RelativePageUri}";
        public bool IsPageOpened => _webDriver.Url.Equals(FullPageUrl);
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
    }
}
