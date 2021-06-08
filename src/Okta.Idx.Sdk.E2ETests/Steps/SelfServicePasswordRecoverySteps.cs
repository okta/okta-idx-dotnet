using Okta.Idx.Sdk.E2ETests.Helpers;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class SelfServicePasswordRecoverySteps : BaseTestSteps
    {

        public SelfServicePasswordRecoverySteps(ITestUserHelper userHelper,
                                                ITestConfig testConfig)
            : base(userHelper, testConfig)
        { }

        [Given(@"an org with an ORG Policy that defines Authenticators with Password and Email as required")]
        public void GivenAnOrgWithAnORGPolicyThatDefinesAuthenticatorsWithPasswordAndEmailAsRequired()
        { }

        [Given(@"a user named ""(.*)""")]
        public async Task GivenAUserNamed(string firstName)
        {
            _testConfig.TestUser = await _userHelper.GetActivePasswordUserAsync(firstName);
        }

        [Given(@"Mary is a user with a verified email and a set password")]
        public void GivenMaryIsAUserWithAVerifiedEmailAndASetPasswordAsync()
        { }

    }
}
