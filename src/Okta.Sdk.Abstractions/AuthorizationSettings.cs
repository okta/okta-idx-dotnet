// <copyright file="AuthorizationSettings.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace Okta.Sdk.Abstractions
{
    public class AuthorizationSettings
    {
        /// <summary>
        /// Gets or sets the Authorization header scheme.
        /// </summary>
        public AuthorizationType AuthorizationType { get; set; } = AuthorizationType.None;

        /// <summary>
        /// Gets or sets the Authorization header value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets the default Authorization Settings.
        /// </summary>
        /// <returns>The default Authorization Settings.</returns>
        public static AuthorizationSettings GetDefault()
                => new AuthorizationSettings { AuthorizationType = AuthorizationType.None };

        /// <summary>
        /// Returns the base64 encoded clientId:clientSecret.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <param name="clientSecret">The client Secret.</param>
        /// <returns>The encoded client credentials.</returns>
        public static string EncodeClientCredentials(string clientId, string clientSecret)
        {
            var clientCredentialsBytes = Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}");

            return Convert.ToBase64String(clientCredentialsBytes);
        }
    }
}
