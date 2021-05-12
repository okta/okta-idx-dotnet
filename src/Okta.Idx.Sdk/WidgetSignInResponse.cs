// <copyright file="WidgetSignInResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents a response to a request to start a widget interaction.
    /// </summary>
    public class WidgetSignInResponse
    {
        /// <summary>
        /// Gets or sets the IDX context.
        /// </summary>
        public IIdxContext IdxContext { get; set; }

        /// <summary>
        /// Gets or sets the sign-in widget configuration.
        /// </summary>
        public SignInWidgetConfiguration SignInWidgetConfiguration { get; set; }
    }
}
