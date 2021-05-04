using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    public class SelectAuthenticatorMethodViewModel
    {
        public string Profile { get; set; }

        public string EnrollmentId { get; set; }

        public string AuthenticatorId { get; set; }

    }
}