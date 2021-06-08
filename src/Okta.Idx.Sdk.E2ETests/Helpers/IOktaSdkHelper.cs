using Okta.Sdk;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public interface IOktaSdkHelper
    {
        Task<IUser> CreateActiveUserIdentifiedWithPasswordAsync(string userName, string firstName, string password);
        Task<IUser> CreateUnassignedUserIdentifiedWithPasswordAsync(string userName, string password);
        Task<IUser> CreateActiveUserAndAddToGroupAsync(string userName, string firstName, string password, string groupName);
        Task DeleteUserAsync(string email);
    }
}
