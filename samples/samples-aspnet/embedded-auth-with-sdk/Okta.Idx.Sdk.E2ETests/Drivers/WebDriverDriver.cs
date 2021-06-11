using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace embedded_auth_with_sdk.E2ETests.Drivers
{
    public class WebDriverDriver : IDisposable
    {
        private Lazy<IWebDriver> _webDriver = new(() => SetupWebDriver());
        private bool _disposed = false;

        public IWebDriver WebDriver => _webDriver.Value;

        private static IWebDriver SetupWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--start-maximized");            
            options.AddArgument("--disable-notifications");
            options.AddArgument("--no-sandbox");
            return new ChromeDriver(options);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_webDriver.IsValueCreated)
                {
                    WebDriver.Close();
                    WebDriver.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
