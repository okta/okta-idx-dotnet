using A18NAdapter;
using BoDi;
using Okta.Idx.Sdk.E2ETests.Drivers;
using Okta.Idx.Sdk.E2ETests.Helpers;
using System.Diagnostics;
using TechTalk.SpecFlow;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

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
            var a18nAdapter = new A18nAdapter(config.A18nApiKey);

            _container.RegisterInstanceAs<ITestConfig>(config);
            _container.RegisterInstanceAs<IA18nAdapter>(a18nAdapter, dispose: true);
            _container.RegisterInstanceAs<IWebServerDriver>(_webServerDriver, dispose: true);
            _container.RegisterTypeAs<OktaSdkHelper, IOktaSdkHelper>(); 
            _container.RegisterTypeAs<TestUserHelper, ITestUserHelper>();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _webServerDriver.StopWebServer();
        }

        #region Before & After test run
        [BeforeTestRun]
        public static void BeforeTestRunInjection()
        { }

        [AfterTestRun]
        public static void AfterTestRun()
        { }
        #endregion Before & After test run
    }
}
