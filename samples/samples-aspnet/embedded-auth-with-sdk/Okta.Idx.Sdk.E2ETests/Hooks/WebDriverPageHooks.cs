using BoDi;
using System;
using Xunit;
using TechTalk.SpecFlow;
using embedded_auth_with_sdk.E2ETests.Drivers;
using embedded_auth_with_sdk.E2ETests.Helpers;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace embedded_auth_with_sdk.E2ETests.Hooks
{

    [Binding]
    public class WebDriverPageHooks
    {
        private IObjectContainer _container;

        private static IWebServerDriver _webServerDriver;
        private static ITestConfiguration _config;
        private const string DefaultProfileTag = "okta-idx-dotnet";

        public WebDriverPageHooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _container.RegisterInstanceAs<ITestConfiguration>(_config);
            _container.RegisterTypeAs<A18nClientHelper, IA18nClientHelper>();
            _container.RegisterTypeAs<OktaSdkHelper, IOktaSdkHelper>();
            _container.RegisterTypeAs<TestContext, ITestContext>();
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext, ITestContext testContext)
        {
            if (scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
            {
                var time = DateTime.UtcNow;
                var name = $"{scenarioContext.ScenarioInfo.Title}-{time.Day:D2}-{time.Hour:D2}-{time.Minute:D2}-{time.Second:D2}";

                testContext.TakeScreenshot(name);
            }
        }

        #region Before & After test run
        [BeforeTestRun]
        public static void BeforeTestRunInjection(IISWebServerDriver webServerDriver)
        {
            _webServerDriver = webServerDriver;
            SetUpTestEnvironment(webServerDriver);
        }

        private static void SetUpTestEnvironment(IISWebServerDriver webServerDriver)
        {
            webServerDriver.StartWebServer();
            _config = ConfigBuilder.Configuration;
            _config.SiteUrl = webServerDriver.SiteUrl;
            if (string.IsNullOrEmpty(_config.A18nProfileTag))
            {
                _config.A18nProfileTag = DefaultProfileTag;
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _webServerDriver.StopWebServer();
        }
        #endregion Before & After test run
    }
}
