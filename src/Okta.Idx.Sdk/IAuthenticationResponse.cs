// <copyright file="IAuthenticationResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// This interface represents the authentication response.
    /// </summary>
    public interface IAuthenticationResponse : IResource
    {
        /// <summary>
        /// Gets the Token Info.
        /// </summary>
        ITokenResponse TokenInfo { get; }

        /// <summary>
        /// Gets the authentication status.
        /// </summary>
        AuthenticationStatus AuthenticationStatus { get; }

        /// <summary>
        /// Gets the IDX context.
        /// </summary>
        IIdxContext IdxContext { get; }

        /// <summary>
        /// Gets the authenticators.
        /// </summary>
        IList<IAuthenticator> Authenticators { get; }

        /// <summary>
        /// Gets current authenticator enrollment.
        /// </summary>
        IAuthenticator CurrentAuthenticatorEnrollment { get; }

        /// <summary>
        /// Gets the current authenticator.
        /// </summary>
        IAuthenticator CurrentAuthenticator { get; }

        /// <summary>
        /// Gets a value indicating whether response contains 'skip' remediation option.
        /// </summary>
        bool CanSkip { get; }

        /// <summary>
        /// Gets a message to user
        /// </summary>
        string MessageToUser { get; }
    }
}
