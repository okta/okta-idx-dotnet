namespace Okta.Idx.Sdk.E2ETests
{
<<<<<<< HEAD:src/Okta.Idx.Sdk.E2ETests/Configuration.cs
    public class Configuration : IConfiguration
=======
    public class TestConfiguration : ITestConfiguration
>>>>>>> remotes/origin/master:src/Okta.Idx.Sdk.E2ETests/TestConfiguration.cs
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
