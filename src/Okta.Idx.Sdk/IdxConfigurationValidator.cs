// <copyright file="IdxConfigurationValidator.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.Idx.Sdk.Configuration
{
    /// <summary>
    /// Helper validator class for OktaClient settings
    /// </summary>
    public class IdxConfigurationValidator
    {
        /// <summary>
        /// Validates the Okta Identity Engine configuration
        /// </summary>
        /// <param name="configuration">The configuration to be validated</param>
        public static void Validate(IdxConfiguration configuration)
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

            if (string.IsNullOrEmpty(configuration.RedirectUri) || configuration.RedirectUri.IndexOf("{redirectUri}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentNullException(
                    nameof(configuration.RedirectUri),
                    "Your Okta Application redirect URI is missing.You can find it in the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}
