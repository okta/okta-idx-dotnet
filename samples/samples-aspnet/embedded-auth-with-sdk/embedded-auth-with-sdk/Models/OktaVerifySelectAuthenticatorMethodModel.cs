using Okta.Idx.Sdk.OktaVerify;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifySelectAuthenticatorMethodModel
    {
        public OktaVerifySelectAuthenticatorMethodModel(OktaVerifyAuthenticationOptions oktaVerifyAuthenticationOptions, string pollEndpoint = "/OktaVerify/ChallengePoll")
        {   
            this.OktaVerifyAuthenticationOptions = oktaVerifyAuthenticationOptions;
            this.PollEndpoint = pollEndpoint;
        }

        public OktaVerifyAuthenticationOptions OktaVerifyAuthenticationOptions { get; }

        public string PollEndpoint { get; set; }

        public int? RefreshInterval { get; set; }
    }
}
