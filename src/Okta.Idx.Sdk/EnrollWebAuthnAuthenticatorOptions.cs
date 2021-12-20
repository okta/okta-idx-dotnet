// <copyright file="EnrollWebAuthnAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class EnrollWebAuthnAuthenticatorOptions
    {
        public string Attestation { get; set; }

        public string ClientData { get; set; }
    }
}
