// <copyright file="VerifyWebAuthnAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// The options to verify a Web Authn authenticator
    /// </summary>
    public class VerifyWebAuthnAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the authenticator data
        /// </summary>
        public string AuthenticatorData { get; set; }

        /// <summary>
        /// Gets or sets the client data
        /// </summary>
        public string ClientData { get; set; }

        /// <summary>
        /// Gets or sets the signature data
        /// </summary>
        public string SignatureData { get; set; }
    }
}
