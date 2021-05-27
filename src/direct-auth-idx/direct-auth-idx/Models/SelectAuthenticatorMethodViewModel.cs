using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    using Okta.Idx.Sdk;

    public class SelectAuthenticatorMethodViewModel
    {
        public string Profile { get; set; }

        public string EnrollmentId { get; set; }

        public string AuthenticatorId { get; set; }

        public AuthenticatorMethodType MethodType { get; set; }

        public IList<AuthenticatorMethodType> MethodTypes { get; set; }

    }
}