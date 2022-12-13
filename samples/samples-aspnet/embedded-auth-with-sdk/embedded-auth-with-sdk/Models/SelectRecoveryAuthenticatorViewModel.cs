using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace embedded_auth_with_sdk.Models
{
    public class SelectRecoveryAuthenticatorViewModel
    {
        [Required]
        [Display(Name = "Authenticator ID")]
        public string AuthenticatorId { get; set; }

        public IList<AuthenticatorViewModel> Authenticators { get; set; }

    }
}