// <copyright file="IAuthenticator.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;
using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to reprenset an authenticator.
    /// </summary>
    public interface IAuthenticator : IResource
    {
        /// <summary>
        /// Gets the display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        IList<IAuthenticatorMethod> Methods { get; }
    }
}
