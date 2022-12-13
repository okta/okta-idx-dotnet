using System.Collections.Generic;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifySelectEnrollmentChannelOptionModel
    {
        static OktaVerifySelectEnrollmentChannelOptionModel()
        {
            LabelTexts = new Dictionary<string, string>
            {
                { "email", "Email me a setup link" },
                { "sms", "Text me a setup link" }
            };
        }

        public static Dictionary<string, string> LabelTexts { get; }

        public OktaVerifySelectEnrollmentChannelOptionModel()
        { 
        }

        public OktaVerifySelectEnrollmentChannelOptionModel(string value)
        {
            this.Value = value;
        }

        public string LabelText => LabelTexts[Value];

        public string Value { get; }

        public string Name => "SelectedChannel";

    }
}
