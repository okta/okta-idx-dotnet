namespace Okta.Idx.Sdk.E2ETests
{
    public interface ITestConfig
    {
        public string NormalUser { get; set; }
        public string DeactivatedUser { get; set; }
        public string LockedUser { get; set; }
        public string SuspendedUser { get; set; }
        public string UnassignedUser { get; set; }
        public string UserPassword { get; set; }
    }
}