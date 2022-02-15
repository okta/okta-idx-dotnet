using System.ComponentModel.DataAnnotations;

namespace embedded_auth_with_sdk.Models
{
    public class OktaVerifyEnrollWithEmailModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
