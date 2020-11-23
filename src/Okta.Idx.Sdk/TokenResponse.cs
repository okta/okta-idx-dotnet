using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class TokenResponse : Resource, ITokenResponse
    {
        public string TokenType => GetStringProperty("token_type");

        public int? ExpiresIn => GetProperty<int?>("expires_in");

        public string AccessToken => GetStringProperty("access_token");

        public string RefreshToken => GetStringProperty("refresh_token");

        public string IdToken => GetStringProperty("id_token");

        public string Scope => GetStringProperty("scope");
    }
}
