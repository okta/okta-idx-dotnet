using embedded_sign_in_widget_e2etests.Drivers;
using FluentAssertions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.PageObjectModels
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
