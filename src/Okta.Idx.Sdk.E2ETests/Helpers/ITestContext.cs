using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests
{
    public interface ITestContext
    {
        UserProfile UserProfile { get; }
        Task SetActivePasswordUserAsync(string firstName);
        Task SetUnenrolledUserAsync(string firstName);
        Task SetActivePasswordAndEmailUserAsync(string firstName);
        Task SetActivePasswordAndSmsUserAsync(string firstName);
        Task<string> GetRecoveryCodeFromEmail();
        Task<string> GetRecoveryCodeFromSms();
        Task EnrollPhoneAuthenticator();
    }
}
