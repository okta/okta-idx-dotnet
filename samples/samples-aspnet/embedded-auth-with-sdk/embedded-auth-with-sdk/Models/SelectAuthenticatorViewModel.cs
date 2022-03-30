using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    
    public class SelectAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Authenticator is required")]
        [Display(Name = "Authenticator ID")]
        public string AuthenticatorId { get; set; }
        
        public string PasswordId { get; set; }

        public string PhoneId { get; set; }

        public string WebAuthnId { get; set; }

        public string TotpId { get; set; }
        
        public string OktaVerifyId { get; set; }
        public string SecurityQuestionId { get; set; }
        
        public bool CanSkip { get; set; }

        public IList<AuthenticatorViewModel> Authenticators { get; set; }

        public bool IsPasswordSelected => PasswordId != null && PasswordId == AuthenticatorId;

        public bool IsPhoneSelected => PhoneId != null && PhoneId == AuthenticatorId;

        public bool IsWebAuthnSelected => WebAuthnId != null && WebAuthnId == AuthenticatorId;

        public bool IsTotpSelected => TotpId != null && TotpId == AuthenticatorId;
        public bool IsSecurtiyQuestionSelected => SecurityQuestionId != null && SecurityQuestionId == AuthenticatorId;

        public bool IsOktaVerifySelected => OktaVerifyId != null && OktaVerifyId == AuthenticatorId;
    }
}
