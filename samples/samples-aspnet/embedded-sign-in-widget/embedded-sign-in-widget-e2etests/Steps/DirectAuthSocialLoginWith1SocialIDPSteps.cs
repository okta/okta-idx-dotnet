using embedded_sign_in_widget_e2etests.Helpers;
using TechTalk.SpecFlow;
namespace embedded_sign_in_widget_e2etests.Steps
{
        [Binding]
        public class DirectAuthSocialLoginWith1SocialIDPSteps : BaseTestSteps
        {
            public DirectAuthSocialLoginWith1SocialIDPSteps(ITestContext context) : base(context)
            { }
            [Given(@"a configured IDP object for Facebook")]
            public void GivenAConfiguredIDPObjectForFacebook()
            { }

            [Given(@"a user named Mary does not have an account in the org but has a Facebook account")]
            public void GivenMaryDoesNotHaveAnAccountInTheOrgButHasAFacebookAccount()
            {
                _context.SetUnenrolledUserWithFacebookAccount();
            }
        }
    }
