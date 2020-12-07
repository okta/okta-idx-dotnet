// <copyright file="IdxConfiguration.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

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
        public List<string> Scopes { get; set; } = OktaDefaults.Scopes;

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        public string RedirectUri { get; set; }

        public bool IsConfidentialClient
        {
            get
            {
                var isConfidentialClient = false;

                if (!string.IsNullOrEmpty(ClientSecret) && ClientSecret.IndexOf("{ClientSecret}", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    isConfidentialClient = true;
                }

                return isConfidentialClient;
            }
        }
    }
}
