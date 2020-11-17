using Okta.IdentityEngine.Sdk.Configuration;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.IdentityEngine.Sdk.Configuration
{
    public class OktaConfigurationConverter
    {
        /// <summary>
        /// Converts an OktaIdentityEngineConfiguration into an OktaClientConfiguration.
        /// </summary>
        /// <param name="configuration">The Okta Idx configuration.</param>
        /// <returns>The Okta client configuration.</returns>
        public static OktaClientConfiguration Convert(OktaIdentityEngineConfiguration configuration)
        {
            return new OktaClientConfiguration
            {
                OktaDomain = UrlHelper.GetOktaDomain(configuration.Issuer),
            };
        }
    }
}
