using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace direct_auth_idx.Models
{
    public class SelectRecoveryAuthenticatorViewModel
    {
        [Required]
        [Display(Name = "Authenticator ID")]
        public string AuthenticatorId { get; set; }
        public IList<AuthenticatorViewModel> Authenticators { get; set; }

    }
}