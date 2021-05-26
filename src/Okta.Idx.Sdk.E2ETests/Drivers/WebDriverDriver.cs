using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Okta.Idx.Sdk.E2ETests.Drivers
{
    public class WebDriverDriver
    {
        private Lazy<IWebDriver> _webDriver = new(() => SetupWebDriver());

        public IWebDriver WebDriver => _webDriver.Value;

        private static IWebDriver SetupWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            return new ChromeDriver(options);
        }
    }
}
