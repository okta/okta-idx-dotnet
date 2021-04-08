using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    public class SelectAuthenticatorViewModel
    {
        public string AuthenticatorId { get; set; }

        public IList<AuthenticatorViewModel> Authenticators { get; set; }
    }
}