// <copyright file="SignInWidgetConfiguration.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using Newtonsoft.Json;
using Okta.Idx.Sdk.Configuration;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents configuration information used to initialize the sign-in widget.
    /// </summary>
    public class SignInWidgetConfiguration
    {
        /// <summary>
        /// The default version.
        /// </summary>
        public const string DefaultVersion = "5.5.2";

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInWidgetConfiguration"/> class.
        /// </summary>
        /// <param name="idxConfiguration">The IDX configuration.</param>
        /// <param name="idxContext">The IDX context.</param>
        /// <param name="version">The widget version to use.</param>
        public SignInWidgetConfiguration(IdxConfiguration idxConfiguration, IIdxContext idxContext, string version = DefaultVersion)
        {
            this.UseInteractionCodeFlow = true;
            this.Version = version ?? DefaultVersion;

            this.BaseUrl = idxConfiguration?.Issuer?.Split(new string[] { "/oauth2" }, StringSplitOptions.RemoveEmptyEntries)[0];
            this.ClientId = idxConfiguration?.ClientId;
            this.RedirectUri = idxConfiguration?.RedirectUri;
            this.AuthParams = new SignInWidgetAuthParams(idxConfiguration);

            this.InteractionHandle = idxContext?.InteractionHandle;
            this.State = idxContext?.State;
            this.CodeChallenge = idxContext?.CodeChallenge;
            this.CodeChallengeMethod = idxContext?.CodeChallengeMethod;
        }

        /// <summary>
        /// Gets or sets the interaction handle.
        /// </summary>
        [JsonProperty("interactionHandle")]
        public string InteractionHandle { get; set; }

        /// <summary>
        /// Gets or sets the widget version.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI
        /// </summary>
        [JsonProperty("redirectUri")]
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the auth params.
        /// </summary>
        [JsonProperty("authParams")]
        public SignInWidgetAuthParams AuthParams { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use interaction code flow.
        /// </summary>
        [JsonProperty("useInteractionCodeFlow")]
        public bool UseInteractionCodeFlow { get; set; }

        /// <summary>
        /// Gets or sets the state handle.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the code challenge.
        /// </summary>
        [JsonProperty("codeChallenge")]
        public string CodeChallenge { get; set; }

        /// <summary>
        /// Gets or sets the code challenge method.
        /// </summary>
        [JsonProperty("codeChallengeMethod")]
        public string CodeChallengeMethod { get; set; }
    }
}
