using Okta.Idx.Sdk.OktaVerify;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyEnrollPollModel
    {
        public OktaVerifyEnrollPollModel(OktaVerifyEnrollOptions oktaVerifyEnrollOptions, string pollEndpoint = "/OktaVerify/EnrollPoll")
        {   
            this.OktaVerifyEnrollOptions = oktaVerifyEnrollOptions;
            this.QrCode = oktaVerifyEnrollOptions.QrCode;
            this.PollEndpoint = pollEndpoint;
            this.RefreshInterval = oktaVerifyEnrollOptions.Refresh;
            this.Message = "When prompted, tap Scan a QR code, then scan the QR code below:";
        }

        public OktaVerifyEnrollOptions OktaVerifyEnrollOptions { get; }

        public string QrCode { get; set; }

        public string PollEndpoint { get; set; }

        public int? RefreshInterval { get; set; }

        public string Message { get; set; }
    }
}
