// <copyright file="EnrollPhoneAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    public class EnrollPhoneAuthenticatorOptions : SelectEnrollAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the method type.
        /// </summary>
        public AuthenticatorMethodType MethodType { get; set; }
    }
}
