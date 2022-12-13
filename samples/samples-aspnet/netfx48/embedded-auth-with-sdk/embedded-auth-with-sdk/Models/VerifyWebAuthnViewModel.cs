using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class VerifyWebAuthnViewModel
    {
        public string Challenge { get; set; }

        public string WebAuthnCredentialId { get; set; }

        public string ClientData { get; set; }

        public string AuthenticatorData { get; set; }

        public string SignatureData { get; set; }
    }
}