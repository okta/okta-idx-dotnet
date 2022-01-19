// <copyright file="AuthenticationResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// This class represents the authentication response.
    /// </summary>
    public class AuthenticationResponse : Resource, IAuthenticationResponse
    {
        /// <summary>
        /// Gets the Token Info.
        /// </summary>
        public ITokenResponse TokenInfo { get; internal set; }

        /// <summary>
        /// Gets the authentication status.
        /// </summary>
        public AuthenticationStatus AuthenticationStatus { get; internal set; }

        /// <summary>
        /// Gets the IDX context.
        /// </summary>
        public IIdxContext IdxContext { get; internal set; }

        /// <summary>
        /// Gets the authenticators.
        /// </summary>
        public IList<IAuthenticator> Authenticators { get; internal set; }

        /// <summary>
        /// Gets the current authenticator enrollment.
        /// </summary>
        public IAuthenticator CurrentAuthenticatorEnrollment { get; internal set; }

        /// <summary>
        /// Gets the current authenticator.
        /// </summary>
        public IAuthenticator CurrentAuthenticator { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether response contains 'skip' remediation option.
        /// </summary>
        public bool CanSkip { get; internal set; }

        /// <summary>
        /// Gets the message to user
        /// </summary>
        public string MessageToUser { get; internal set; }
    }
}
