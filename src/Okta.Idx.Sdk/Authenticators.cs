// <copyright file="Authenticators.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A class to represent the authenticators object from the IDX response.
    /// </summary>
    public class Authenticators : Resource, IAuthenticators
    {
        /// <inheritdoc/>
        public string Type => GetStringProperty("type");

        /// <inheritdoc/>
        public IList<IAuthenticatorValue> Value => GetArrayProperty<IAuthenticatorValue>("value");
    }
}
