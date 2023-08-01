using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public interface IPasswordWarnStateResolver
    {
        bool IsInPasswordWarnState(IIdxResponse authenticationResponse);
    }
}
