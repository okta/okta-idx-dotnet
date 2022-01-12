using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.OktaVerify
{
    public class OktaVerifyAuthenticatorValue : AuthenticatorValue
    {
        public IOktaVerifyContextualData ContextualData => GetProperty<OktaVerifyContextualData>("contextualData");
    }
}
