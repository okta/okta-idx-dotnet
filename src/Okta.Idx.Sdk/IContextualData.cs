// <copyright file="IContextualData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent ContextualData type.
    /// </summary>
    public interface IContextualData : IResource
    {
        /// <summary>
        /// Gets the QR Code.
        /// </summary>
        IQrCode QrCode { get; }

        /// <summary>
        /// Gets the Shared Secret.
        /// </summary>
        string SharedSecret { get; }
    }
}
