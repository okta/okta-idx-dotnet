// <copyright file="IdxRequestPayload.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// The IDX request payload.
    /// </summary>
    public class IdxRequestPayload : Resource
    {
        /// <summary>
        /// Gets or sets the state handle
        /// </summary>
        public string StateHandle
        {
            get => GetStringProperty("stateHandle");
            set => this["stateHandle"] = value;
        }
    }
}
