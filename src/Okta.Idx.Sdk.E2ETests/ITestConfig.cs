namespace Okta.Idx.Sdk.E2ETests
{
    public interface ITestConfig
    {
        string NormalUser { get; set; }
        string DeactivatedUser { get; set; }
        string LockedUser { get; set; }
        string SuspendedUser { get; set; }
        string UnassignedUser { get; set; }
        string UserPassword { get; set; }
        string A18nApiKey { get; set; }
        string A18nProfileId { get; set; }
        int IisPort { get; set; }
    }
}
