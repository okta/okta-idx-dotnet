using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    using System.ComponentModel.DataAnnotations;

    using Okta.Idx.Sdk;

    public class SelectAuthenticatorMethodViewModel
    {
        public string Profile { get; set; }

        public string EnrollmentId { get; set; }

        public string AuthenticatorId { get; set; }

        [Required]
        [Display(Name = "Method Type")]
        public string MethodType { get; set; }

        public IList<AuthenticatorMethodType> MethodTypes { get; set; }

    }
}