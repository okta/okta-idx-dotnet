using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public interface ITestUserHelper
    {
        Task<TestUserProperties> GetActivePasswordUserAsync();
        Task<TestUserProperties> GetUnassignedUser();
        Task<string> GetRecoveryCodeFromEmail();
    }
}
