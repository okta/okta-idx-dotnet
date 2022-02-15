namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyEnrollPollModel
    {
        public OktaVerifyEnrollPollModel() 
        {
            this.PollEndpoint = "/OktaVerify/EnrollPoll";
            this.RefreshInterval = 4000;
        }

        public OktaVerifyEnrollPollModel(string qrCode, string pollEndpoint = "/OktaVerify/EnrollPoll")
        {
            this.QrCode = qrCode;
            this.PollEndpoint = pollEndpoint;
            this.RefreshInterval = 4000;
            this.Message = "When prompted, tap Scan a QR code, then scan the QR code below:";
        }

        public string QrCode { get; set; }

        public string PollEndpoint { get; set; }

        public int? RefreshInterval { get; set; }

        public string Message { get; set; }
    }
}
