using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class FalsePasswordWarnStateResolver : IPasswordWarnStateResolver
    {
        public bool IsInPasswordWarnState(IIdxResponse authenticationResponse)
        {
            return false;
        }
    }
}
