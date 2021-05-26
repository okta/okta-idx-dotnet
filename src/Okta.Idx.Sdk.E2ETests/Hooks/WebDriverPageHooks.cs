using BoDi;
using Okta.Idx.Sdk.E2ETests.Drivers;
using System.Threading;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Hooks
{
    [Binding]
    public class WebDriverPageHooks
    {
        private IWebServerDriver _webServerDriver;
        private IObjectContainer _container;

        public WebDriverPageHooks(IObjectContainer container, IISWebServerDriver webServerDriver)
        {
            _container = container;
            _webServerDriver = webServerDriver;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _webServerDriver.StartWebServer();

            var config = ConfigBuilder.Configuration;
            _container.RegisterInstanceAs<ITestConfig>(config);

            _container.RegisterInstanceAs<IWebServerDriver>(_webServerDriver);
        }

        [AfterScenario]
        public void AfterScenario(WebDriverDriver wdd)
        {
            var webDriver = wdd.WebDriver;

            _webServerDriver.StopWebServer();
            
            webDriver.Close();
            webDriver.Dispose();

        }

        #region Before & After test run
        [BeforeTestRun]
        public static void BeforeTestRunInjection(ITestRunnerManager testRunnerManager, ITestRunner testRunner)
        {
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
        }
        #endregion Before & After test run
    }
}
