using Okta.Idx.Sdk;
using Okta.Idx.Sdk.OktaVerify;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifySelectAuthenticatorMethodModel
    {
        public OktaVerifySelectAuthenticatorMethodModel()
        {
            PollEndpoint = "/OktaVerify/ChallengePoll";
        }

        public OktaVerifySelectAuthenticatorMethodModel(OktaVerifyAuthenticationOptions oktaVerifyAuthenticationOptions, string pollEndpoint = "/OktaVerify/ChallengePoll")
        {   
            this.OktaVerifyAuthenticationOptions = oktaVerifyAuthenticationOptions;
            this.PollEndpoint = pollEndpoint;
        }

        public OktaVerifySelectAuthenticatorMethodModel(IAuthenticator authenticator, string pollEndpoint = "/OktaVerify/ChallengePoll")
        {
            this.AuthenticatorId = authenticator.Id;
            this.PollEndpoint = pollEndpoint;
        }

        public string AuthenticatorId { get; set; }

        public string MethodType { get; set; }

        public OktaVerifyAuthenticationOptions OktaVerifyAuthenticationOptions { get; }

        public string PollEndpoint { get; set; }

        public int? RefreshInterval { get; set; }
    }
}
