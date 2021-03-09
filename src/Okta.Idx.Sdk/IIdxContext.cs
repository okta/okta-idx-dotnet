// <copyright file="IIdxContext.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the IDX context.
    /// </summary>
    public interface IIdxContext
    {
        /// <summary>
        /// Gets the PKCE code verifier used during the interact call.
        /// </summary>
        string CodeVerifier { get; }

        /// <summary>
        /// Gets the PKCE code challenge used during the interact call.
        /// </summary>
        string CodeChallenge { get; }

        /// <summary>
        /// Gets the PKCE code challenge method used during the interact call.
        /// </summary>
        string CodeChallengeMethod { get; }

        /// <summary>
        /// Gets the interaction handle obtained during the interact call.
        /// </summary>
        string InteractionHandle { get; }

        /// <summary>
        /// Gets the state used during the interact call.
        /// </summary>
        string State { get; }
    }
}
