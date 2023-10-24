// <copyright file="PasswordWarnStateResolver.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

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
