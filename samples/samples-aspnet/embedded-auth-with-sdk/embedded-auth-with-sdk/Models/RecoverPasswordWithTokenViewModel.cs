using System.ComponentModel.DataAnnotations;

namespace embedded_auth_with_sdk.Models
{
    public class RecoverPasswordWithTokenViewModel
    {

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
    }
}
