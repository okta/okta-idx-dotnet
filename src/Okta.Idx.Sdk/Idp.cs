// <copyright file="Idp.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class Idp : Resource, IIdp
    {
        /// <inheritdoc/>
        public string Id => GetStringProperty("id");

        /// <inheritdoc/>
        public string Name => GetStringProperty("name");
    }
}
