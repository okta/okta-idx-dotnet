using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Helpers
{
    public interface ITestContext
    {
        TestUserProfile TestUserProfile { get; set; }
        Task SetActivePasswordUserAsync();
        void SetUnenrolledUserWithFacebookAccount();
    }
}
