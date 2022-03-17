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
        
        public string PasswordId { get; private set; }

        public string PhoneId { get; private set; }

        public string WebAuthnId { get; private set; }

        public string TotpId { get; private set; }
        
        public string OktaVerifyId { get; private set; }
        
        public bool CanSkip { get; set; }

        private IList<AuthenticatorViewModel> _authenticators;
        public IList<AuthenticatorViewModel> Authenticators
        {
            get => _authenticators;
            
            set
            {
                _authenticators = value;
                AuthenticatorId = _authenticators?.FirstOrDefault()?.AuthenticatorId;
                PasswordId = _authenticators?.FirstOrDefault(x => x.Name.ToLower() == "password")?.AuthenticatorId;
                PhoneId = _authenticators?.FirstOrDefault(x => x.Name.ToLower() == "phone")?.AuthenticatorId;
                WebAuthnId = _authenticators?.FirstOrDefault(x => x.Name.ToLower() == "security key or biometric")
                    ?.AuthenticatorId;
                TotpId = _authenticators?.FirstOrDefault(x => x.Name.ToLower() == "google authenticator")
                    ?.AuthenticatorId;
                OktaVerifyId = _authenticators?.FirstOrDefault(x => x.Name.ToLower() == "okta verify")?.AuthenticatorId;
            }
        }

        public bool IsPasswordSelected => PasswordId != null && PasswordId == AuthenticatorId;

        public bool IsPhoneSelected => PhoneId != null && PhoneId == AuthenticatorId;

        public bool IsWebAuthnSelected => WebAuthnId != null && WebAuthnId == AuthenticatorId;

        public bool IsTotpSelected => TotpId != null && TotpId == AuthenticatorId;

        public bool IsOktaVerifySelected => OktaVerifyId != null && OktaVerifyId == AuthenticatorId;
    }
}
