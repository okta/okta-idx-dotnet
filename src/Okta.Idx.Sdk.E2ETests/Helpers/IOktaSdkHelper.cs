using Okta.Sdk;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public interface IOktaSdkHelper
    {
        Task<IUser> CreateActiveUser(string userName, string phone, string firstName, string password);
        Task AddUserToGroup(IUser user, string groupName);
        Task DeleteUserAsync(string email);
        Task<IUserFactor> EnrollPhoneFactor(string emailAddress, string phoneNumber);
        Task ActivateFactor(IUserFactor factor, string emailAddress, string passCode);
    }
}
