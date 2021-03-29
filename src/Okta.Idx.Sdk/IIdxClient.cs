// <copyright file="IIdxClient.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A client to interact with the IDX API.
    /// </summary>
    public interface IIdxClient : IOktaClient
    {
        /// <summary>
        /// Gets the client configuration.
        /// </summary>
        IdxConfiguration Configuration { get; }

        /// <summary>
        /// Changes user's password.
        /// </summary>
        /// <param name="changePasswordOptions">The change password options</param>
        /// <param name="idxContext">The IDX context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication response.</returns>
        Task<AuthenticationResponse> ChangePasswordAsync(ChangePasswordOptions changePasswordOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authenticates a user with username/password credentials
        /// </summary>
        /// <param name="authenticationOptions">The authentication topions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication response.</returns>
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationOptions authenticationOptions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates the password recovery flow
        /// </summary>
        /// <param name="recoverPasswordOptions">The password recovery options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication response.</returns>
        Task<AuthenticationResponse> RecoverPasswordAsync(RecoverPasswordOptions recoverPasswordOptions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verify an authenticator
        /// </summary>
        /// <param name="verifyAuthenticatorOptions">The options to verify an authenticator.</param>
        /// <param name="idxContext">The IDX context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication response.</returns>
        Task<AuthenticationResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorOptions verifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);
    }
}
