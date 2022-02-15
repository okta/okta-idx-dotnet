// <copyright file="IAuthenticatorMethod.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents an authenticator's method type.
    /// </summary>
    public interface IAuthenticatorMethod : IResource
    {
        /// <summary>
        /// Gets the type
        /// </summary>
        AuthenticatorMethodType Type { get; }
    }
}
