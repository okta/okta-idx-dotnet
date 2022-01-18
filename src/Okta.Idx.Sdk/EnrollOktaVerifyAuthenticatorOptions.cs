using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class EnrollOktaVerifyAuthenticatorOptions : SelectEnrollAuthenticatorOptions
    {
        public string Channel { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
