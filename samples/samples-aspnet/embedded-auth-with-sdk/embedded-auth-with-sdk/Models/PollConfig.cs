using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class PollConfig
    {
        public IRemediationOption RemediationOption { get; set; }
        public int RefreshInterval { get; set; }
        public string StateHandle { get; set; }

        public EnrollOktaVerifyAuthenticatorViewModel ViewModel { get; set; }
    }
}