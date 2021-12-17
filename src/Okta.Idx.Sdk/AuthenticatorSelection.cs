// <copyright file="AuthenticatorSelection.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class AuthenticatorSelection : Resource, IAuthenticatorSelection
    {
        public bool? RequireResidentKey => GetBooleanProperty("requireResidentKey");

        public string UserVerification => GetStringProperty("userVerification");
    }
}
