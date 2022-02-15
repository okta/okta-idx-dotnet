using System.ComponentModel.DataAnnotations;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyEnterCodeModel
    {
        public OktaVerifyEnterCodeModel()
        { 
        }
        
        [Required]
        [Display(Name = "Enter code from Okta Verify app")]
        public string Code { get; set; }
    }
}
