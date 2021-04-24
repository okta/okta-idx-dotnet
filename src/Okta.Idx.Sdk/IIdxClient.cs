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

        /// <summary>
        /// Revoke tokens. Revoking an access token doesn't revoke the associated refresh token. However, revoking a refresh token does revoke the associated access token.
        /// </summary>
        /// <see href="https://developer.okta.com/docs/guides/revoke-tokens/revokeatrt/"/>
        /// <param name="tokenType">The token type.</param>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task RevokeTokensAsync(TokenType tokenType, string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="userProfile">The user profile. Contains all the dynamic properties required for registration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication response.</returns>
        Task<AuthenticationResponse> RegisterAsync(UserProfile userProfile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Trigger the authenticator enrollment flow.
        /// </summary>
        /// <param name="enrollAuthenticatorOptions">The options for enrollment.</param>
        /// <param name="idxContext">The IDX context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication response.</returns>
        Task<AuthenticationResponse> EnrollAuthenticatorAsync(EnrollAuthenticatorOptions enrollAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts the password recovery process with the recovery authenticator.
        /// </summary>
        /// <param name="selectAuthenticatorOptions">The options to choose authenticator</param>
        /// <param name="idxContext">The IDX context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication response.</returns>
        Task<AuthenticationResponse> SelectRecoveryAuthenticatorAsync(SelectAuthenticatorOptions selectAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default);
    }
}
