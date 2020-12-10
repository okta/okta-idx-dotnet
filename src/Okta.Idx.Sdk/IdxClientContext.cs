// <copyright file="IdxClientContext.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    public class IdxClientContext : IIdxClientContext
    {
        private readonly string _codeVerifier;
        private readonly string _codeChallenge;
        private readonly string _codeChallengeMethod;

        public IdxClientContext()
        {
        }

        public IdxClientContext(string codeVerifier, string codeChallenge, string codeChallengeMethod)
        {
            _codeVerifier = codeVerifier;
            _codeChallenge = codeChallenge;
            _codeChallengeMethod = codeChallengeMethod;
        }

        public string CodeVerifier => _codeVerifier;

        public string CodeChallenge => _codeChallenge;

        public string CodeChallengeMethod => _codeChallengeMethod;
    }
}
