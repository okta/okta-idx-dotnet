using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class AuthenticationResponse : Resource
    {
        // TODO: Create a TokenInfo class
        public ITokenResponse TokenInfo { get; set; }

        public AuthenticationStatus AuthenticationStatus { get; set; }

        public IIdxContext IdxContext { get; set; }
    }
}
