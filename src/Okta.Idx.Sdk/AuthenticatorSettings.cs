using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class AuthenticatorSettings : Resource, IAuthenticatorSettings
    {
        /// <inheritdoc/>
        public IComplexity Complexity => GetResourceProperty<Complexity>("complexity");

        /// <inheritdoc/>
        public IAge Age => GetResourceProperty<Age>("age");

        /// <inheritdoc/>
        public int? DaysToExpiry => GetIntegerProperty("daysToExpiry");
    }
}
