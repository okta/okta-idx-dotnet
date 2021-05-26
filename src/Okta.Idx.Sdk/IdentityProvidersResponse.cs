// <copyright file="IdentityProviderResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents available third party identity providers.
    /// </summary>
    public class IdentityProvidersResponse
    {
        /// <summary>
        /// Gets the state handle for the IDX context.
        /// </summary>
        public string State { get => Context?.State; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public IIdxContext Context { get; set; }

        /// <summary>
        /// Gets or sets the IDP options.
        /// </summary>
        public List<IdpOption> IdpOptions { get; set; }
    }
}
