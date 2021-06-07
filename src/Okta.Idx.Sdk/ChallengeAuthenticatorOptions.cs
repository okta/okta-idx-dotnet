// <copyright file="ChallengeAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Options required to challenge an authenticator.
    /// </summary>
    public class ChallengeAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the enrollment ID.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the authenticator ID.
        /// </summary>
        public string AuthenticatorId { get; set; }
    }
}
