// <copyright file="AuthenticatorMethodType.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "N/A")]
    public sealed class AuthenticatorMethodType : StringEnum
    {
        /// <summary>
        /// SMS
        /// </summary>
        public static AuthenticatorMethodType Sms = new AuthenticatorMethodType("sms");

        /// <summary>
        /// Voice
        /// </summary>
        public static AuthenticatorMethodType Voice = new AuthenticatorMethodType("voice");

        /// <summary>
        /// Email
        /// </summary>
        public static AuthenticatorMethodType Email = new AuthenticatorMethodType("email");

        /// <summary>
        /// Voice
        /// </summary>
        public static AuthenticatorMethodType Password = new AuthenticatorMethodType("password");

        /// <summary>
        /// Implicit operator declaration to accept and convert a string value as a <see cref="AuthenticatorMethodType"/>
        /// </summary>
        /// <param name="value">The value to use</param>
        public static implicit operator AuthenticatorMethodType(string value) => new AuthenticatorMethodType(value);

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatorMethodType"/> class.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public AuthenticatorMethodType(string value)
            : base(value)
        {
        }
    }
}
