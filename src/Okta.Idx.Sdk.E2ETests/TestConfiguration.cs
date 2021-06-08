namespace Okta.Idx.Sdk.E2ETests
{
    public class TestConfiguration : ITestConfiguration
    {
        public string UserPassword { get; set; }
        public string A18nApiKey { get; set; }
        public string A18nProfileId { get; set; }
        public string A18nProfileTag { get; set; }
        public int IisPort { get; set; }
        public string SiteUrl { get; set; }
        public string MfaRequiredGroup { get; set; }
        public string PhoneEnrollmentRequiredGroup { get; set; }
    }
}
