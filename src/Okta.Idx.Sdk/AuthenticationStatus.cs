// <copyright file="AuthenticationStatus.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// This enum represents all the possible authentication response statuses
    /// </summary>
    public enum AuthenticationStatus
    {
        /// <summary>
        /// Success
        /// </summary>
        Success,

        /// <summary>
        /// Password expired
        /// </summary>
        PasswordExpired,

        /// <summary>
        /// Waiting for authenticator verification
        /// </summary>
        AwaitingAuthenticatorVerification,

        /// <summary>
        /// Waiting for password reset
        /// </summary>
        AwaitingPasswordReset,

        /// <summary>
        /// Waiting for authenticator enrollment
        /// </summary>
        AwaitingAuthenticatorEnrollment,

        /// <summary>
        /// Waiting for authenticator enrollment data.
        /// </summary>
        AwaitingAuthenticatorEnrollmentData,

        /// <summary>
        /// Waiting for an authenticator to continue with the authentication process.
        /// </summary>
        AwaitingAuthenticatorSelection,

        /// <summary>
        /// Waiting for the authenticator data to continue with the authentication process.
        /// </summary>
        AwaitingChallengeAuthenticatorData,

        /// <summary>
        /// Waiting for an authenticator selection to continue with the authentication process.
        /// </summary>
        AwaitingChallengeAuthenticatorSelection,
    }
}
