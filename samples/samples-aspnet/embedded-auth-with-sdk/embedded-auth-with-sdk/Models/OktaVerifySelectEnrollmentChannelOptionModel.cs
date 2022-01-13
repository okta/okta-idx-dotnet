using Okta.Idx.Sdk.OktaVerify;
using System.Collections.Generic;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifySelectEnrollmentChannelOptionModel
    {
        static OktaVerifySelectEnrollmentChannelOptionModel()
        {
            LabelTexts = new Dictionary<string, string>
            {
                { "qrcode", "Scan a QR code" },
                { "email", "Email me a setup link" },
                { "sms", "Text me a setup link" }
            };
        }
        public static Dictionary<string, string> LabelTexts { get; }

        public OktaVerifySelectEnrollmentChannelOptionModel()
        { 
        }

        public OktaVerifySelectEnrollmentChannelOptionModel(OktaVerifyRemediationParameter option)
        {
            this.Value = option?.Value;
        }

        public string LabelText => LabelTexts[Value];

        public string Value { get; }

        public string Name => "SelectedChannel";

    }
}
