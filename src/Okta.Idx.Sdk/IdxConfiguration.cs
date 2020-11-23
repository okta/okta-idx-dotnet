using Okta.Sdk.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.Configuration
{
    public class IdxConfiguration 
    {
        /// <summary>
        /// Gets or sets the client ID for your application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client Secret for your application. Optional.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets a list of string based scopes.
        /// </summary>
        public IList<string> Scopes { get; set; } = OktaDefaults.Scopes;

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        public string Issuer { get; set; }
    }
}
