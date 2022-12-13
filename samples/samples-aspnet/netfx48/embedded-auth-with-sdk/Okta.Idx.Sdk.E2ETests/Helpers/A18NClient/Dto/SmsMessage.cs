using System;

namespace embedded_auth_with_sdk.E2ETests.Helpers.A18NClient.Dto
{
    public class SmsMessage
    {
        public string MessageId { get; set; }
        public string ProfileId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }
}
