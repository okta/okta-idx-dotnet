using System;
using System.Collections.Generic;
using System.Text;

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
