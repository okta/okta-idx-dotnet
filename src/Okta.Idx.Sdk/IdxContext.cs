﻿// <copyright file="IdxContext.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// The IDX Context.
    /// </summary>
    public class IdxContext : IIdxContext
    {
        private string _codeVerifier;
        private string _codeChallenge;
        private string _codeChallengeMethod;
        private string _interactionHandle;
        private string _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdxContext"/> class.
        /// </summary>
        public IdxContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdxContext"/> class.
        /// </summary
        /// <param name="codeVerifier">The PKCE code verifier.</param>
        /// <param name="codeChallenge">The PKCE code challenge.</param>
        /// <param name="codeChallengeMethod">The PKCE code challenge method.</param>
        /// <param name="interactionHandle">The interaction handle.</param>
        /// <param name="state">The state.</param>
        public IdxContext(string codeVerifier, string codeChallenge, string codeChallengeMethod, string interactionHandle, string state)
        {
            _codeVerifier = codeVerifier;
            _codeChallenge = codeChallenge;
            _codeChallengeMethod = codeChallengeMethod;
            _interactionHandle = interactionHandle;
            _state = state;
        }

        /// <summary>
        /// Gets the PKCE code verifier
        /// </summary>
        public string CodeVerifier { get => _codeVerifier; set => _codeVerifier = value; }

        /// <summary>
        /// Gets the PKCE code challenge
        /// </summary>
        public string CodeChallenge { get => _codeChallenge; set => _codeChallenge = value; }

        /// <summary>
        /// Gets the PKCE code challenge method
        /// </summary>
        public string CodeChallengeMethod { get => _codeChallengeMethod; set => _codeChallengeMethod = value; }

        /// <summary>
        /// Gets the interaction handle
        /// </summary>
        public string InteractionHandle { get => _interactionHandle; set => _interactionHandle = value; }

        /// <summary>
        /// Gets the state
        /// </summary>
        public string State { get => _state; set => _state = value; }
    }
}
