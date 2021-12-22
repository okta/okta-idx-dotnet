// <copyright file="ActivationData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc cref="IActivationData"/>
    public class ActivationData : Resource, IActivationData
    {
        /// <inheritdoc/>
        public string Attestation => GetStringProperty("attestation");

        /// <inheritdoc/>
        public IAuthenticatorSelection AuthenticatorSelection =>
            GetResourceProperty<AuthenticatorSelection>("authenticatorSelection");

        /// <inheritdoc/>
        public string Challenge => GetStringProperty("challenge");

        /// <inheritdoc/>
        public IList<IPublicKeyCredParam> PublicKeyCredParams =>
            GetArrayProperty<IPublicKeyCredParam>("pubKeyCredParams");

        /// <inheritdoc/>
        public IUser User => GetResourceProperty<User>("user");

        /// <inheritdoc/>
        public IU2fParams U2fParams => GetResourceProperty<U2fParams>("u2fParams");
    }
}
