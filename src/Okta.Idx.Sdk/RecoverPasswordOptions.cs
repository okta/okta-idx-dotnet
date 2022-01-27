﻿// <copyright file="RecoverPasswordOptions.cs" company="Okta, Inc">
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

        /// <summary>
        /// Gets or sets the recovery token.
        /// The property must be used together with the Passcode property.
        /// Use user API, ForgotPasswordGenerateOneTimeTokenAsync function to get the the recovery URL containing the token.
        /// With the function parameter, you can control whether the URL is returned as part of the response or sent in the recovery email.
        /// </summary>
        public string RecoveryToken { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// The property should be used in conjunction with RecoveryToken property.
        /// </summary>
        public string Passcode { get; set; }
    }
}
