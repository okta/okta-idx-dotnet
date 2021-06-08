using BoDi;
using embedded_sign_in_widget_e2etests.Drivers;
using embedded_sign_in_widget_e2etests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_sign_in_widget_e2etests.Hooks
{
    [Binding]
    public class WebDriverPageHooks
    {
        private IObjectContainer _container;
        private static IWebServerDriver _webServerDriver;
        private static ITestConfiguration _config;

        public WebDriverPageHooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _container.RegisterInstanceAs<ITestConfiguration>(_config);
            _container.RegisterTypeAs<OktaSdkHelper, IOktaSdkHelper>();
            _container.RegisterTypeAs<TestContext, ITestContext>();
        }

        [BeforeTestRun]
        public static void BeforeTestRunInjection(IISWebServerDriver webServerDriver)
        {
            _webServerDriver = webServerDriver;
            _config = ConfigBuilder.Configuration;
            _config.SiteUrl = webServerDriver.SiteUrl;
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _webServerDriver.StopWebServer();
        }
    }
}
