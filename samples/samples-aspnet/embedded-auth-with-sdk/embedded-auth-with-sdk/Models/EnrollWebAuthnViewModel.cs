using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class EnrollWebAuthnViewModel : BaseViewModel
    {
        public string Attestation { get; set; }

        public string ClientData { get; set; }

        public string Challenge { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

    }
}