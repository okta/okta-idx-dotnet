// <copyright file="OktaVerifyEnrollmentChannel.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk.OktaVerify
{
    /// <summary>
    /// Okta Verify enrollment channel options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "N/A")]
    public sealed class OktaVerifyEnrollmentChannel : StringEnum
    {
        /// <summary>
        /// qrcode.
        /// </summary>
        public static OktaVerifyEnrollmentChannel QrCode = new OktaVerifyEnrollmentChannel("qrcode");

        /// <summary>
        /// email.
        /// </summary>
        public static OktaVerifyEnrollmentChannel Email = new OktaVerifyEnrollmentChannel("email");

        /// <summary>
        /// sms.
        /// </summary>
        public static OktaVerifyEnrollmentChannel Sms = new OktaVerifyEnrollmentChannel("sms");

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaVerifyEnrollmentChannel"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public OktaVerifyEnrollmentChannel(string value) : base(value)
        {
        }
    }
}
