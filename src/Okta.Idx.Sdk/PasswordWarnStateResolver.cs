using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class PasswordWarnStateResolver : IPasswordWarnStateResolver
    {
        public bool IsInPasswordWarnState(IIdxResponse authenticationResponse)
        {
            throw new NotImplementedException();
        }

        public static IPasswordWarnStateResolver Default
        {
            get
            {
                return new PasswordWarnStateResolver();
            }
        }
    }
}
