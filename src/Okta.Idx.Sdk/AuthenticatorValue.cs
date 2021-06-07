// <copyright file="AuthenticatorValue.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// This class represents an Authenticator.
    public class AuthenticatorValue : Resource, IAuthenticatorValue
    {
        /// <inheritdoc/>
        public string DisplayName => GetStringProperty("displayName");

        /// <inheritdoc/>
        public string Id => GetStringProperty("id");

        /// <inheritdoc/>
        public string Key => GetStringProperty("key");

        /// <inheritdoc/>
        public IList<IAuthenticatorMethod> Methods => GetArrayProperty<IAuthenticatorMethod>("methods");
    }
}
