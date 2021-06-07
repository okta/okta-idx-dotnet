// <copyright file="AuthenticatorEnrollments.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class AuthenticatorEnrollments : Resource, IAuthenticatorEnrollments
    {
        /// <inheritdoc/>
        public string Type => GetStringProperty("type");

        /// <inheritdoc/>
        public IList<IAuthenticatorEnrollment> Value => GetArrayProperty<IAuthenticatorEnrollment>("value");
    }
}
