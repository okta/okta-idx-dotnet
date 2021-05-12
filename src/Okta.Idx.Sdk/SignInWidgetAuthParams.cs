// <copyright file="SignInWidgetAuthParams.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Newtonsoft.Json;
using Okta.Idx.Sdk.Configuration;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents the auth params passed to the widget.
    /// </summary>
    public class SignInWidgetAuthParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignInWidgetAuthParams"/> class.
        /// </summary>
        public SignInWidgetAuthParams()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInWidgetAuthParams"/> class.
        /// </summary>
        /// <param name="idxConfiguration">The IDX configuration.</param>
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
