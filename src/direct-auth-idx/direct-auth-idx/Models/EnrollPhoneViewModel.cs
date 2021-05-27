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
        [Required]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Method Type")]
        public string MethodType { get; set; }

        public IList<AuthenticatorMethodType> MethodTypes { get; set; }
    }
}