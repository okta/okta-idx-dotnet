using Newtonsoft.Json;
using Okta.Idx.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class SignInWidgetAuthParams
    {
        public SignInWidgetAuthParams()
        {
        }

        public SignInWidgetAuthParams(IdxConfiguration idxConfiguration)
        {
            Issuer = idxConfiguration.Issuer;
            Scopes = idxConfiguration.Scopes;
        }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }
    }
}
