using System;
using TechTalk.SpecFlow;

namespace embedded_auth_with_sdk.E2ETests.Steps
{
    [Binding]
    public class _1_2LoginWithIdentifierFirstStepDefinitions
    {
        [Given(@"a Global Session Policy defines the Primary factor as Password / IDP / any factor allowed by app sign on rules")]
        public void GivenAGlobalSessionPolicyDefinesThePrimaryFactorAsPasswordIDPAnyFactorAllowedByAppSignOnRules()
        {
        }

        [Given(@"the Global Session Policy does NOT require a second factor")]
        public void GivenTheGlobalSessionPolicyDoesNOTRequireASecondFactor()
        {
        }

        [Given(@"a SPA, WEB APP or MOBILE with an Authentication Policy that is defined as Any (.*) factor")]
        public void GivenASPAWEBAPPOrMOBILEWithAnAuthenticationPolicyThatIsDefinedAsAnyFactor(int p0)
        {
        }

        [Given(@"User Enumeration Prevention is set to ENABLED in Security > General")]
        public void GivenUserEnumerationPreventionIsSetToENABLEDInSecurityGeneral()
        {
        }

        [Given(@"the list of Authenticators contains Email and Password is optional")]
        public void GivenTheListOfAuthenticatorsContainsEmailAndPasswordIsOptional()
        {
        }
    }
}
