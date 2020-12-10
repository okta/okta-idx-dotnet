// <copyright file="OktaConfigurationConverter.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace Okta.Idx.Sdk.Configuration
{
    public class OktaConfigurationConverter
    {
        /// <summary>
        /// Converts an OktaIdentityEngineConfiguration into an OktaClientConfiguration.
        /// </summary>
        /// <param name="configuration">The Okta Idx configuration.</param>
        /// <returns>The Okta client configuration.</returns>
        public static OktaClientConfiguration Convert(IdxConfiguration configuration)
        {
            return new OktaClientConfiguration
            {
                OktaDomain = UrlHelper.GetOktaRootUrl(configuration.Issuer),
            };
        }
    }
}
