using System.ComponentModel.DataAnnotations;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyEnrollWithPhoneNumberModel
    {
        [Required]
        [Display(Name = "country code")]
        public string CountryCode { get; set; }

        [Required]
        [Display(Name = "phone number")]
        public string PhoneNumber { get; set; }
    }
}
