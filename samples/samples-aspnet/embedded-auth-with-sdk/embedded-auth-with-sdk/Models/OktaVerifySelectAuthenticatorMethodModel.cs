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
    }
}
