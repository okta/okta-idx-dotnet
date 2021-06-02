using A18NClient;
using A18NClient.Dto;
using BoDi;
using Okta.Idx.Sdk.E2ETests.Drivers;
using Okta.Idx.Sdk.E2ETests.Helpers;
using TechTalk.SpecFlow;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Okta.Idx.Sdk.E2ETests.Hooks
{

    [Binding]
    public class WebDriverPageHooks
    {
        private IObjectContainer _container;

        private static IWebServerDriver _webServerDriver;
        private static TestConfig _config;
        private static A18nClient _a18nClient;

        public WebDriverPageHooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _container.RegisterInstanceAs<ITestConfig>(_config);
            _container.RegisterInstanceAs<IA18nClient>(_a18nClient);
            _container.RegisterTypeAs<OktaSdkHelper, IOktaSdkHelper>(); 
            _container.RegisterTypeAs<TestUserHelper, ITestUserHelper>();
        }

        [AfterScenario]
        public void AfterScenario()
        { }

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
            if (string.IsNullOrWhiteSpace(_config.A18nProfileId))
            {
                _a18nClient = new A18nClient(_config.A18nApiKey, createNewDefaultProfile: true, "okta-idx-dotnet");
            }
            else
            {
                _a18nClient = new A18nClient(_config.A18nApiKey, _config.A18nProfileId);
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _a18nClient.Dispose();

            _webServerDriver.StopWebServer();
        }
        #endregion Before & After test run
    }
}
