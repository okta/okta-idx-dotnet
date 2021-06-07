using System;
using System.IO;
using FlexibleConfiguration;

namespace Okta.Idx.Sdk.E2ETests
{
    internal static class ConfigBuilder 
    {
        private static Lazy<Configuration> _testConfig = new Lazy<Configuration>(() => BuildProperties());
        internal static Configuration Configuration => _testConfig.Value;

        private static Configuration BuildProperties()
        {
            string configurationFileRoot = Directory.GetCurrentDirectory();
            var applicationAppSettingsLocation = Path.Combine(configurationFileRoot ?? string.Empty, "settings.json");

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(applicationAppSettingsLocation, optional: true)
                .AddEnvironmentVariables("okta_testing", "_", root: "okta:testing");


            var compiledConfig = new Configuration();
            configBuilder.Build().GetSection("okta").GetSection("testing").Bind(compiledConfig);
            configBuilder.Build().Bind(compiledConfig);

            return compiledConfig;
        }
    }
}
