// <copyright file="IQrCode.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An Interface to represent QR Code type
    /// </summary>
    public interface IQrCode : IResource
    {
        /// <summary>
        /// Gets the Method
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the Href
        /// </summary>
        string Href { get; }

        /// <summary>
        /// Gets the Type
        /// </summary>
        string Type { get; }
    }
}
