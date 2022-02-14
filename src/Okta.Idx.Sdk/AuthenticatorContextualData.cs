﻿// <copyright file="AuthenticatorContextualData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <inheritdoc cref="IAuthenticatorContextualData"/>
    public class AuthenticatorContextualData : Resource, IAuthenticatorContextualData
    {
        /// <inheritdoc />
        public IActivationData ActivationData => GetResourceProperty<ActivationData>("activationData");

        /// <inheritdoc />
        public IChallengeData ChallengeData => GetResourceProperty<ChallengeData>("challengeData");

        /// <inheritdoc/>
        public IQrCode QrCode => GetResourceProperty<QrCode>("qrcode");

        /// <inheritdoc/>
        public string SharedSecret => GetStringProperty("sharedSecret");
    }
}
