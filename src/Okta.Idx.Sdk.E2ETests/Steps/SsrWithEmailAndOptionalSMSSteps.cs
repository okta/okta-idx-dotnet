using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Okta.Idx.Sdk.E2ETests.Steps
{
    [Binding]
    public class SsrWithEmailAndOptionalSMSSteps : BaseTestSteps
    {

        public SsrWithEmailAndOptionalSMSSteps(ITestContext context)
            : base(context)
        { }

        [Given(@"a Profile Enrollment policy defined assigning new users to the Everyone Group and by collecting ""(.*)"", ""(.*)"", and ""(.*)"", is allowed and assigned to a SPA, WEB APP or MOBILE application")]
        public void GivenAProfileEnrollmentPolicyDefinedAssigningNewUsersToTheEveryoneGroupAndByCollectingAndIsAllowedAndAssignedToASPAWEBAPPOrMOBILEApplication(string p0, string p1, string p2)
        { }

        [Given(@"""(.*)"" is selected for Email Verification under Profile Enrollment in Security > Profile Enrollment")]
        public void GivenIsSelectedForEmailVerificationUnderProfileEnrollmentInSecurityProfileEnrollment(string p0)
        { }

        [Given(@"configured Authenticators are Password \(required\), Email \(required\), and SMS \(optional\)")]
        public void GivenConfiguredAuthenticatorsArePasswordRequiredEmailRequiredAndSMSOptional()
        { }

        [Given(@"Mary does not have an account in the org")]
        public async Task GivenMaryDoesNotHaveAnAccountInTheOrg()
        {
            await _context.SetUnenrolledUserAsync("Mary");
        }
    }
}
