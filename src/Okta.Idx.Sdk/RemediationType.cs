// <copyright file="RemediationType.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A class to represent all the remediation types.
    /// </summary>
    public static class RemediationType
    {
        /// <summary>
        /// identify
        /// </summary>
        public static readonly string Identify = "identify";

        /// <summary>
        /// reenroll-authenticator
        /// </summary>
        public static readonly string ReenrollAuthenticator = "reenroll-authenticator";

        /// <summary>
        /// challenge-authenticator
        /// </summary>
        public static readonly string ChallengeAuthenticator = "challenge-authenticator";

        /// <summary>
        /// This is not a remediation itself, but it's used internally to identify that a response is successful.
        /// </summary>
        public static readonly string SuccessWithInteractionCode = "successWithInteractionCode";
    }
}
