namespace Okta.Idx.Sdk.E2ETests
{
<<<<<<< HEAD:src/Okta.Idx.Sdk.E2ETests/IConfiguration.cs
    public interface IConfiguration
=======
    public interface ITestConfiguration
>>>>>>> remotes/origin/master:src/Okta.Idx.Sdk.E2ETests/ITestConfiguration.cs
    {
        string UserPassword { get; set; }
        string A18nApiKey { get; set; }
        string A18nProfileId { get; set; }
        string A18nProfileTag { get; set; }
        int IisPort { get; set; }
        string SiteUrl { get; set; }
        string MfaRequiredGroup { get; set; }
        string PhoneEnrollmentRequiredGroup { get; set; }
    }
}
