﻿using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps
{
    [Binding]
    public class SelfServicePasswordRecoverySteps : BaseTestSteps
    {

        public SelfServicePasswordRecoverySteps(ITestContext context)
            : base(context)
        { }

        [Given(@"an org with an ORG Policy that defines Authenticators with Password and Email as required")]
        public void GivenAnOrgWithAnORGPolicyThatDefinesAuthenticatorsWithPasswordAndEmailAsRequired()
        { }

        [Given(@"a user named ""(.*)""")]
        public void GivenAUserNamed(string firstName)
        {
        }

        [Given(@"Mary is a user with a verified email and a set password")]
        public async Task GivenMaryIsAUserWithAVerifiedEmailAndASetPasswordAsync()
        {
            await _context.SetActivePasswordUserAsync("Mary");
        }

    }
}
