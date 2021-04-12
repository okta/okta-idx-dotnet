// <copyright file="AuthenticationStatus.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
