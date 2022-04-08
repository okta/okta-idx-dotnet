// <copyright file="IChallengeData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents the challenge data of a Web Authn authenticator.
    /// </summary>
    public interface IChallengeData : IResource
    {
        /// <summary>
        /// Gets the challenge.
        /// </summary>
        string Challenge { get; }

        /// <summary>
        /// Gets the user verification.
        /// </summary>
        string UserVerification { get; }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        Resource Extensions { get; }
    }
}
