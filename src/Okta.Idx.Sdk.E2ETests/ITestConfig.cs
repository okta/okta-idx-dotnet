namespace Okta.Idx.Sdk.E2ETests
{
    public interface ITestConfig
    {
        string UserPassword { get; set; }
        string A18nApiKey { get; set; }
        string A18nProfileId { get; set; }
        int IisPort { get; set; }
        string SiteUrl { get; set; }
    }
}
