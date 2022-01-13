using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.OktaVerify
{
    /// <summary>
    /// Okta Verify specific authenticator.
    /// </summary>
    public class OktaVerifyAuthenticatorValue : AuthenticatorValue
    {
        /// <summary>
        /// Gets the contextual data.
        /// </summary>
        public IOktaVerifyContextualData ContextualData => GetProperty<OktaVerifyContextualData>("contextualData");
    }
}
