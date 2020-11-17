using Okta.IdentityEngine.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.IdentityEngine.Sdk.Configuration
{
    /// <summary>
    /// Helper validator class for OktaClient settings
    /// </summary>
    public class OktaIdentityEngineConfigurationValidator
    {
        /// <summary>
        /// Validates the Okta Identity Engine configuration
        /// </summary>
        /// <param name="configuration">The configuration to be validated</param>
        public static void Validate(OktaIdentityEngineConfiguration configuration)
        {
            if (configuration.Issuer.IndexOf("{yourOktaIssuer}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "Replace {yourOktaIssuer} with your Okta domain. You can copy your domain from the Okta Developer Console. Follow these instructions to find it: https://bit.ly/finding-okta-domain", nameof(configuration.Issuer));
            }

            if (string.IsNullOrEmpty(configuration.ClientId))
            {
                throw new ArgumentNullException(
                    nameof(configuration.ClientId),
                    "Your client ID is missing. You can copy it from the Okta Developer Console in the details for the Application you created. Follow these instructions to find it: https://bit.ly/finding-okta-app-credentials");
            }

            if (configuration.ClientId.IndexOf("{ClientId}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentNullException(
                    nameof(configuration.ClientId),
                    "Replace {clientId} with the client ID of your Application. You can copy it from the Okta Developer Console in the details for the Application you created. Follow these instructions to find it: https://bit.ly/finding-okta-app-credentials");
            }

        }


    }
}
