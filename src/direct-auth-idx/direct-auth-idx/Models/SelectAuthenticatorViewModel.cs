﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    using direct_auth_idx.Utilities;

    public class SelectAuthenticatorViewModel
    {
        [RequiredIf(nameof(CanSkip), false, ErrorMessage = "Authenticator ID is required")]
        [Display(Name = "Authenticator ID")]
        public string AuthenticatorId { get; set; }
        
        public string PasswordId { get; set; }

        public string PhoneId { get; set; }
        
        public bool CanSkip { get; set; }

        public IList<AuthenticatorViewModel> Authenticators { get; set; }

        public bool IsPasswordSelected
        {
            get
            {
                return PasswordId == AuthenticatorId;
            }
        }

        public bool IsPhoneSelected
        {
            get
            {
                return PhoneId == AuthenticatorId;
            }
        }
    }
}