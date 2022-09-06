using System.Collections;
using System.Collections.Generic;
using Okta.Idx.Sdk;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifySelectAuthenticatorMethodModel
    {
        public OktaVerifySelectAuthenticatorMethodModel()
        {
            PollEndpoint = "/OktaVerify/ChallengePoll";
        }

        public string AuthenticatorId { get; set; }

        public string MethodType { get; set; }

        public string PollEndpoint { get; set; }

        public int? RefreshInterval { get; set; }

        public IList<AuthenticatorMethodType> MethodTypes { get; set; }

        public string Message { get; set; }

    }
}
