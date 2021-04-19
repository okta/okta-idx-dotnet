using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    public class EnrollPhoneViewModel
    {
        public AuthenticatorMethodType MethodType  => AuthenticatorMethodType.Sms;

        [Required]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}