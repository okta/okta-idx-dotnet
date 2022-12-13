using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class AuthenticatorViewModel
    {
        public string Name { get; set; }

        public string AuthenticatorId { get; set; }

        public string EnrollmentId { get; set; }
    }
}