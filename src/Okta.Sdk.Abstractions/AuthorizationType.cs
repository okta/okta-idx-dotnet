// <copyright file="AuthorizationType.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Sdk.Abstractions
{
    /// <summary>
    /// The Authorization type.
    /// </summary>
    public enum AuthorizationType
    {
        /// <summary>
        /// Authorization is not required.
        /// </summary>
        None,

        /// <summary>
        /// Authorization is Basic.
        /// </summary>
        Basic,

        /// <summary>
        /// Authorization is SSWS.
        /// </summary>
        Ssws,
    }
}
