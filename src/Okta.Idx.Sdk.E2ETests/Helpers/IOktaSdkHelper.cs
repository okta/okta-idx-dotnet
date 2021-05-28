using System.Threading.Tasks;

namespace Okta.Idx.Sdk.E2ETests.Helpers
{
    public interface IOktaSdkHelper
    {
        Task CreateActiveUserIdentifiedWithPassword(string userName, string password);
    }
}