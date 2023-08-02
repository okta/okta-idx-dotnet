using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc />
    public class PasswordWarnStateResolver : IPasswordWarnStateResolver
    {
        /// <inheritdoc />
        public bool IsInPasswordWarnState(IIdxResponse authenticationResponse)
        {
            return authenticationResponse.FindRemediationOption(RemediationType.ReenrollAuthenticatorWarning, false) != null;
        }

        private static IPasswordWarnStateResolver _defaultResolver = new PasswordWarnStateResolver();

        /// <summary>
        /// Gets or sets the default instance.
        /// </summary>
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
