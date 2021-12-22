// <copyright file="User.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc cref="IUser"/>
    public class User : Resource, IUser
    {
        /// <inheritdoc/>
        public string DisplayName => GetStringProperty("displayName");

        /// <inheritdoc/>
        public string Name => GetStringProperty("name");

        /// <inheritdoc/>
        public string Id => GetStringProperty("id");
    }
}
