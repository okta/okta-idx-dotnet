// <copyright file="ChallengePhoneAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Options to challenge phone.
    /// </summary>
    public class ChallengePhoneAuthenticatorOptions : ChallengeAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the method type.
        /// </summary>
        public AuthenticatorMethodType MethodType { get; set; }
    }
}
