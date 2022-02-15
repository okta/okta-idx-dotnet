using Okta.Sdk;
using System;
using System.Threading.Tasks;

namespace embedded_auth_with_sdk.E2ETests.Helpers
{
    public interface IOktaSdkHelper
    {
        Task<IUser> CreateActiveUser(string userName, string phone, string firstName, string password);
        Task AddUserToGroup(IUser user, string groupName);
        Task DeleteUserAsync(string email);
        Task<IUserFactor> EnrollPhoneFactor(string emailAddress, string phoneNumber);
        Task AddGoogleAuthenticator(string emailAddress, Func<string, string> sharedSecretToTotpFunc);
        Task ActivateFactor(IUserFactor factor, string emailAddress, string passCode);
        Task ResendEnrollCode();
    }
}
