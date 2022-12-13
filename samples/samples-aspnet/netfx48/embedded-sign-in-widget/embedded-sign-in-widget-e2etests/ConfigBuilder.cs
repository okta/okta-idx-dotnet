using FlexibleConfiguration;
using System;
using System.IO;

namespace embedded_sign_in_widget_e2etests
{
    internal static class ConfigBuilder
    {
        private static Lazy<TestConfiguration> _testConfig = new Lazy<TestConfiguration>(() => BuildProperties());
        internal static TestConfiguration Configuration => _testConfig.Value;

        private static TestConfiguration BuildProperties()
        {
            string configurationFileRoot = Directory.GetCurrentDirectory();
            var applicationAppSettingsLocation = Path.Combine(configurationFileRoot ?? string.Empty, "settings.json");

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(applicationAppSettingsLocation, optional: true)
                .AddEnvironmentVariables("okta_testing", "_", root: "okta:testing");


            var compiledConfig = new TestConfiguration();
            configBuilder.Build().GetSection("okta").GetSection("testing").Bind(compiledConfig);
            configBuilder.Build().Bind(compiledConfig);

            return compiledConfig;
        }
    }
}
