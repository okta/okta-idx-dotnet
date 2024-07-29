﻿// <copyright file="AuthenticationResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
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
        /// Virtual to support mocking in unit tests
        /// </summary>
        public virtual AuthenticationStatus AuthenticationStatus { get; internal set; }

        /// <summary>
        /// Gets the IDX context.
        /// </summary>
        public IIdxContext IdxContext { get; internal set; }

        /// <summary>
        /// Gets the authenticators.
        /// Virtual to support mocking in unit tests
        /// </summary>
        public virtual IList<IAuthenticator> Authenticators { get; internal set; }

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
        [Obsolete("This property will be removed in the next major release. Use Messages instead.")]
        public string MessageToUser { get; internal set; }

        /// <summary>
        /// Gets the list of messages for the user
        /// </summary>
        public IList<IMessage> Messages { get; internal set; }

        /// <summary>
        /// Gets the correct answer for Okta Verify push challenge if number challenge is required.
        /// </summary>
        public string CorrectAnswer
        {
            get => CurrentAuthenticator?.ContextualData?.CorrectAnswer;
        }
    }
}
