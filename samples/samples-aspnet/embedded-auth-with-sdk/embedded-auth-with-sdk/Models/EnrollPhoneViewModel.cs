using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class EnrollPhoneViewModel
    {
        [Required]
        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+\d*$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Method Type")]
        public string MethodType { get; set; }

        public IList<AuthenticatorMethodType> MethodTypes { get; set; }
    }
}