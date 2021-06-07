// <copyright file="RecoverPasswordOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A class to represent the options required for password recovery.
    /// </summary>
    public class RecoverPasswordOptions
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the authenticator type.
        /// </summary>
        public AuthenticatorType AuthenticatorType { get; set; }
    }
}
