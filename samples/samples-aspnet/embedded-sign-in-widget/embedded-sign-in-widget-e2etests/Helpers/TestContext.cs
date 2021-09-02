using embedded_sign_in_widget_e2etests.Drivers;
using Okta.Sdk;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Helpers
{
    public class TestContext : ITestContext
    {
        private readonly ITestConfiguration _configuration;
        private readonly IOktaSdkHelper _oktaHelper;
        private readonly WebDriverDriver _webDriver;
        private bool _disposed = false;
        private const string DefaultScreenshotFolder = "./screenshots";

        public TestContext(ITestConfiguration configuration, IOktaSdkHelper oktaHelper, WebDriverDriver webDriver)
        {
            _oktaHelper = oktaHelper;
            _configuration = configuration;

            Configuration = ConfigBuilder.Configuration;
        }

        public ITestConfiguration Configuration 
        {
            get;
            set;
        }

        public void TakeScreenshot(string name)
        {
            var screenshotDriver = (ITakesScreenshot)_webDriver.WebDriver;
            var saveFolder = _configuration.ScreenshotsFolder;
            if (string.IsNullOrEmpty(saveFolder))
            {
                saveFolder = DefaultScreenshotFolder;
            }

            Screenshot screenShot = screenshotDriver.GetScreenshot();
            var allowedName = name.Replace(':', '-')
                .Replace(' ', '-')
                .Replace('"', '-');
            var fileName = $"{saveFolder}/{allowedName}.png";
            Directory.CreateDirectory(saveFolder);
            screenShot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
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

            _disposed = true;
        }

    }
}
