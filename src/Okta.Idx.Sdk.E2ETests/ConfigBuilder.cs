using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexibleConfiguration;

namespace Okta.Idx.Sdk.E2ETests
{
    internal static class ConfigBuilder 
    {
        private static Lazy<TestConfig> _testConfig = new Lazy<TestConfig>(() => BuildProperties());
        internal static TestConfig Configuration => _testConfig.Value;

        private static TestConfig BuildProperties()
        {
            string configurationFileRoot = Directory.GetCurrentDirectory();
            var applicationAppSettingsLocation = Path.Combine(configurationFileRoot ?? string.Empty, "settings.json");

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(applicationAppSettingsLocation, optional: true)
                .AddEnvironmentVariables("okta_testing", "_", root: "okta:testing");


            var compiledConfig = new TestConfig();
            configBuilder.Build().GetSection("okta").GetSection("testing").Bind(compiledConfig);
            configBuilder.Build().Bind(compiledConfig);

            return compiledConfig;
        }
    }
}
