// <copyright file="QrCode.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// QR Code type
    /// </summary>
    public class QrCode : Resource, IQrCode
    {
        /// <inheritdoc/>
        public string Method => GetStringProperty("method");

        /// <inheritdoc/>
        public string Href => GetStringProperty("href");

        /// <inheritdoc/>
        public string Type => GetStringProperty("type");
    }
}