using System.Threading.Tasks;

namespace embedded_auth_with_sdk.E2ETests
{
    public interface ITestContext
    {
        UserProfile UserProfile { get; }
        Task SetActivePasswordUserAsync(string firstName);
        Task SetUnenrolledUserAsync(string firstName);
        Task SetActivePasswordAndEmailUserAsync(string firstName);
        Task SetActivePasswordAndSmsUserAsync(string firstName);
        void SetUnenrolledUserWithFacebookAccount(string firstName);
        Task<string> GetRecoveryCodeFromEmail();
        Task<string> GetRecoveryCodeFromSms();
        Task EnrollPhoneAuthenticator();
        void TakeScreenshot(string name);
    }
}
