namespace Okta.Idx.Sdk.E2ETests
{
    public class TestConfig : ITestConfig
    {
        public string UserPassword { get; set; }
        public string A18nApiKey { get; set; }
        public string A18nProfileId { get; set; }
        public int IisPort { get; set; }
        public string SiteUrl { get; set; }
    }
}
