using System;
using System.Threading.Tasks;

namespace embedded_auth_with_sdk.E2ETests
{
    public interface ITestContext
    {
        UserProfile UserProfile { get; }
        string TotpSharedSecret { get; set; }
        Task SetActivePasswordUserAsync(string firstName);
        Task SetUnenrolledUserAsync(string firstName);
        Task SetActivePasswordAndEmailUserAsync(string firstName);
        Task SetActivePasswordAndSmsUserAsync(string firstName);
        Task SetActiveUserWithOktaOidcIdpAccount(string firstName);
        Task SetActiveUserRequiresTotpAsync(string firstName);
        Task<string> GetActivationCodeFromEmail(Action resendRequest = default);
        Task<string> GetActivationCodeFromSms(Action resendRequest = default);
        Task EnrollPhoneAuthenticator();
        void TakeScreenshot(string name);
        void SetMfaOktaSocialIdpUser();
        string GetTOTP();
        void GetGoogleSharedSecretFromQrCodeImage(string base64Image);
        Task EnrollGoogleAuthenticator();
    }
}
