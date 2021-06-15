namespace embedded_auth_with_sdk.E2ETests
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
        public string ScreenshotsFolder { get; set; }
        public string FacebookUserEmail { get; set; }
        public string FacebookUserPassword { get; set; }
        public string FacebookMfaUserEmail { get; set; }
        public string FacebookMfaUserPassword { get; set; }
    }
}
