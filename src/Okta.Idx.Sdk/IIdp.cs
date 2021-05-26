// <copyright file="IIdp.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An Idp.
    /// </summary>
    public interface IIdp : IResource
    {
        /// <summary>
        /// Gets the Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
    }
}
