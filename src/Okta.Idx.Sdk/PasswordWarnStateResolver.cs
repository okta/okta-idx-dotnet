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

        private static IPasswordWarnStateResolver _defaultResolver = new FalsePasswordWarnStateResolver();

        public static IPasswordWarnStateResolver Default
        {
            get
            {
                return _defaultResolver;
            }

            set
            {
                _defaultResolver = value;
            }
        }
    }
}
