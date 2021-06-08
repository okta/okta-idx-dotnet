using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public interface ITestUserHelper
    {
        Task<TestUserProperties> GetActivePasswordUserAsync(string firstName);
        Task<TestUserProperties> GetUnenrolledUserAsync(string firstName);
        Task<TestUserProperties> GetActivePasswordAndEmailUserAsync(string firstName);
        Task<string> GetRecoveryCodeFromEmail();
        Task<string> GetRecoveryCodeFromSms();
    }
}
