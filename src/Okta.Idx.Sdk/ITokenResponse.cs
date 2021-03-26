// <copyright file="ITokenResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the token response.
    /// </summary>
    public interface ITokenResponse : IResource
    {
        /// <summary>
        /// Gets the token type.
        /// </summary>
        string TokenType { get; }

        /// <summary>
        /// Gets the expires in property.
        /// </summary>
        int? ExpiresIn { get; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Gets the ID token.
        /// </summary>
        string IdToken { get; }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        string Scope { get; }
    }
}
