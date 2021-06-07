// <copyright file="AuthenticationOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A class that contains all the required request parameters for performing a primary authentication request.
    /// </summary>
    public class AuthenticationOptions
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password. Optional.
        /// </summary>
        public string Password { get; set; }
    }
}
