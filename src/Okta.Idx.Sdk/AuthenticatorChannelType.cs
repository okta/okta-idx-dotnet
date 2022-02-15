// <copyright file="AuthenticatorChannelType.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "N/A")]
    public sealed class AuthenticatorChannelType : StringEnum
    {
        /// <summary>
        /// SMS
        /// </summary>
        public static AuthenticatorChannelType Sms = new AuthenticatorChannelType("sms");

        /// <summary>
        /// QR Code
        /// </summary>
        public static AuthenticatorChannelType QrCode = new AuthenticatorChannelType("qrcode");

        /// <summary>
        /// SMS
        /// </summary>
        public static AuthenticatorChannelType Email = new AuthenticatorChannelType("email");

        /// <summary>
        /// Implicit operator declaration to accept and convert a string value as a <see cref="AuthenticatorChannelType"/>
        /// </summary>
        /// <param name="value">The value to use</param>
        public static implicit operator AuthenticatorChannelType(string value) => new AuthenticatorChannelType(value);

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatorChannelType"/> class.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public AuthenticatorChannelType(string value)
            : base(value)
        {
        }
    }
}
