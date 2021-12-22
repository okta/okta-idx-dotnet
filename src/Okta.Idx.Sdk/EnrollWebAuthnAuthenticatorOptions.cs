// <copyright file="EnrollWebAuthnAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Options to enroll a WebAuthN authenticator.
    /// </summary>
    public class EnrollWebAuthnAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the attestation
        /// </summary>
        public string Attestation { get; set; }

        /// <summary>
        /// Gets or sets the client data.
        /// </summary>
        public string ClientData { get; set; }
    }
}
