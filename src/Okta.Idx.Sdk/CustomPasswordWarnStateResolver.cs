// <copyright file="CustomPasswordWarnStateResolver.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc />
    public class CustomPasswordWarnStateResolver : IPasswordWarnStateResolver
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPasswordWarnStateResolver"/> class.
        /// </summary>
        public CustomPasswordWarnStateResolver()
            : this((idxResponse) => false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPasswordWarnStateResolver"/> class.
        /// </summary>
        /// <param name="checkPasswordWarnState">The implementation.</param>
        public CustomPasswordWarnStateResolver(Func<IIdxResponse, bool> checkPasswordWarnState)
        {
            this.CheckPasswordWarnState = checkPasswordWarnState;
        }

        /// <summary>
        /// Gets or sets the implementation.
        /// </summary>
        protected Func<IIdxResponse, bool> CheckPasswordWarnState
        {
            get;
            set;
        }

        /// <inheritdoc />
        public bool IsInPasswordWarnState(IIdxResponse authenticationResponse)
        {
            return CheckPasswordWarnState(authenticationResponse);
        }
    }
}
