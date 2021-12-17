// <copyright file="ActivationData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class ActivationData : Resource, IActivationData
    {
        public string Attestation => GetStringProperty("attestation");

        public IAuthenticatorSelection AuthenticatorSelection =>
            GetResourceProperty<AuthenticatorSelection>("authenticatorSelection");

        public string Challenge => GetStringProperty("challenge");

        public IList<IPublicKeyCredParam> PublicKeyCredParams =>
            GetArrayProperty<IPublicKeyCredParam>("publicKeyCredParams");

        public IUser User => GetResourceProperty<User>("user");
    }
}
