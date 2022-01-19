namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyPollResponseModel
    {
        public int? Refresh { get; set; } 
        public bool? ContinuePolling { get; set; }

        public string Next { get; set; }
    }
}
