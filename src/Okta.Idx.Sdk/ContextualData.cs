// <copyright file="ContextualData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// ContextualData type.
    /// </summary>
    public class ContextualData : Resource, IContextualData
    {
        /// <inheritdoc/>
        public IQrCode QrCode => GetResourceProperty<QrCode>("qrcode");

        /// <inheritdoc/>
        public string SharedSecret => GetStringProperty("sharedSecret");
    }
}
