// <copyright file="IdxContext.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    public class IdxContext : IIdxContext
    {
        private readonly string _codeVerifier;
        private readonly string _codeChallenge;
        private readonly string _codeChallengeMethod;
        private readonly string _interactionHandle;
        public IdxContext()
        {
        }

        public IdxContext(string codeVerifier, string codeChallenge, string codeChallengeMethod, string interactionHandle)
        {
            _codeVerifier = codeVerifier;
            _codeChallenge = codeChallenge;
            _codeChallengeMethod = codeChallengeMethod;
            _interactionHandle = interactionHandle;
        }

        public string CodeVerifier => _codeVerifier;

        public string CodeChallenge => _codeChallenge;

        public string CodeChallengeMethod => _codeChallengeMethod;

        public string InteractionHandle => _interactionHandle;
    }
}
