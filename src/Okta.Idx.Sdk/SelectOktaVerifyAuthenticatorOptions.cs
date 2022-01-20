// <copyright file="SelectOktaVerifyAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Options to select Okta Verify.
    /// </summary>
    public class SelectOktaVerifyAuthenticatorOptions : SelectAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the authenticator method type
        /// </summary>
        public AuthenticatorMethodType AuthenticatorMethodType { get; set; }
    }
}
