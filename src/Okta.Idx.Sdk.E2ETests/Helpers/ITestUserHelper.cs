namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public interface ITestUserHelper
    {
        UserProperties GetActivePasswordUser();
        UserProperties GetUnassignedUser();
    }
}