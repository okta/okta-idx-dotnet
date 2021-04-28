using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class EnrollPhoneAuthenticatorOptions : EnrollAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the method type.
        /// </summary>
        public AuthenticatorMethodType MethodType { get; set; }
    }
}
