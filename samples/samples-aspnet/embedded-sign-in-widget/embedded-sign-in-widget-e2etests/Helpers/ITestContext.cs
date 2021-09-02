using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Helpers
{
    public interface ITestContext
    {
        ITestConfiguration Configuration { get; set; }
        void TakeScreenshot(string name);
    }
}
