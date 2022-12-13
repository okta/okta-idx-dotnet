using embedded_auth_with_sdk.E2ETests.Helpers.A18NClient;
using embedded_auth_with_sdk.E2ETests.Helpers.A18NClient.Dto;

namespace embedded_auth_with_sdk.E2ETests.Helpers
{
    public interface IA18nClientHelper
    {
        A18nClient GetClient();
        A18nProfile GetDefaultProfile();
        void CleanUpProfile();
    }
}
