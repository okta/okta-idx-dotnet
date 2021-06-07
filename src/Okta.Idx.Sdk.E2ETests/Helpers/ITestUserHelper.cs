using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public interface ITestUserHelper
    {
        Task<UserProfile> GetActivePasswordUserAsync(string firstName);
        Task<UserProfile> GetUnenrolledUserAsync(string firstName);
        Task<UserProfile> GetActivePasswordAndEmailUserAsync(string firstName);
        Task<UserProfile> GetActivePasswordAndSmsUserAsync(string firstName);
        Task<string> GetRecoveryCodeFromEmail();
        Task<string> GetRecoveryCodeFromSms();
        Task EnrollPhoneAuthenticator();
    }
}
