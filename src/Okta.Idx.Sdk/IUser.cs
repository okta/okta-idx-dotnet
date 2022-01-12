// <copyright file="IUser.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// User's information.
    /// </summary>
    public interface IUser : IResource
    {
        /// <summary>
        /// Gets the display name
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the ID
        /// </summary>
        string Id { get; }
    }
}
