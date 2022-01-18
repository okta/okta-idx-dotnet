using Okta.Idx.Sdk;
using Okta.Idx.Sdk.OktaVerify;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyEnrollPollModel
    {
        public OktaVerifyEnrollPollModel() 
        {
            this.PollEndpoint = "/OktaVerify/EnrollPoll";
            this.RefreshInterval = 400;
        }

        // TODO: remove this constructor
/*        public OktaVerifyEnrollPollModel(OktaVerifyEnrollOptions oktaVerifyEnrollOptions, string pollEndpoint = "/OktaVerify/EnrollPoll")
        {   
            this.OktaVerifyEnrollOptions = oktaVerifyEnrollOptions;
            this.QrCode = oktaVerifyEnrollOptions.QrCode;
            this.PollEndpoint = pollEndpoint;
            this.RefreshInterval = oktaVerifyEnrollOptions.Refresh;
            this.Message = "When prompted, tap Scan a QR code, then scan the QR code below:";
        }*/

        public OktaVerifyEnrollPollModel(string qrCode, string pollEndpoint = "/OktaVerify/EnrollPoll")
        {
            this.QrCode = qrCode;
            this.PollEndpoint = pollEndpoint;
            this.RefreshInterval = 400;
            this.Message = "When prompted, tap Scan a QR code, then scan the QR code below:";
        }

        public OktaVerifyEnrollOptions OktaVerifyEnrollOptions { get; }

        public string QrCode { get; set; }

        public string PollEndpoint { get; set; }

        public int? RefreshInterval { get; set; }

        public string Message { get; set; }
    }
}
