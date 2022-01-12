// <copyright file="ContextualData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk.OktaVerify
{
    /// <summary>
    /// ContextualData type.
    /// </summary>
    public class OktaVerifyContextualData : Resource, IOktaVerifyContextualData
    {
        /// <inheritdoc/>
        public IQrCode QrCode => GetResourceProperty<QrCode>("qrcode");

        /// <inheritdoc/>
        public string SelectedChannel => GetStringProperty("selectedChannel");
    }
}
