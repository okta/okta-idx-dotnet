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
        /// identify-recovery
        /// </summary>
        public static readonly string IdentifyRecovery = "identify-recovery";

        /// <summary>
        /// reenroll-authenticator
        /// </summary>
        public static readonly string ReenrollAuthenticator = "reenroll-authenticator";

        /// <summary>
        /// challenge-authenticator
        /// </summary>
        public static readonly string ChallengeAuthenticator = "challenge-authenticator";

        /// <summary>
        /// select-authenticator-authenticate
        /// </summary>
        public static readonly string SelectAuthenticatorAuthenticate = "select-authenticator-authenticate";

        /// <summary>
        /// reset-authenticator
        /// </summary>
        public static readonly string ResetAuthenticator = "reset-authenticator";

        /// <summary>
        /// select-enroll-profile
        /// </summary>
        public static readonly string SelectEnrollProfile = "select-enroll-profile";

        /// <summary>
        /// enroll-profile
        /// </summary>
        public static readonly string EnrollProfile = "enroll-profile";

        /// <summary>
        /// select-authenticator-enroll
        /// </summary>
        public static readonly string SelectAuthenticatorEnroll = "select-authenticator-enroll";

        /// <summary>
        /// enroll-authenticator
        /// </summary>
        public static readonly string EnrollAuthenticator = "enroll-authenticator";

        /// <summary>
        /// skip
        /// </summary>
        public static readonly string Skip = "skip";

        /// <summary>
        /// authenticator-enrollment-data
        /// </summary>
        public static readonly string AuthenticatorEnrollmentData = "authenticator-enrollment-data";

        /// <summary>
        /// authenticator-verification-data
        /// </summary>
        public static readonly string AuthenticatorVerificationData = "authenticator-verification-data";

        /// <summary>
        /// This is not a remediation itself, but it's used internally to identify that a response is successful.
        /// </summary>
        public static readonly string SuccessWithInteractionCode = "successWithInteractionCode";

        /// <summary>
        /// redirect-idp
        /// </summary>
        public static readonly string RedirectIdp = "redirect-idp";

        /// <summary>
        /// This is not a remediation itself, but it's used internally to identify unknown remediations.
        /// </summary>
        public static readonly string Unknown = "unknown";
    }
}
