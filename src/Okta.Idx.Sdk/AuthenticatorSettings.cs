using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class AuthenticatorSettings : Resource, IAuthenticatorSettings
    {
        public IComplexity Complexity => GetResourceProperty<Complexity>("complexity");

        public IAge Age => GetResourceProperty<Age>("age");

        public int? DaysToExpiry => GetIntegerProperty("daysToExpiry");
    }
}
