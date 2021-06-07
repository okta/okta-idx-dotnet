namespace Okta.Idx.Sdk
{
    using System.Collections;
    using System.Collections.Generic;

    public class EnrollPhoneAuthenticatorOptions : SelectEnrollAuthenticatorOptions
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
