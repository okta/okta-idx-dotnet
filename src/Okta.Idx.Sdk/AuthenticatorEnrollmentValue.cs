// <copyright file="AuthenticatorEnrollmentValue.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    public class AuthenticatorEnrollmentValue : Resource, IAuthenticatorEnrollmentValue
    {
        public string DisplayName => GetStringProperty("displayName");

        public string Id => GetStringProperty("id");

        public string Key => GetStringProperty("key");

        public IList<IAuthenticatorEnrollmentMethod> Methods => GetArrayProperty<IAuthenticatorEnrollmentMethod>("methods");

        public IRecover Recover => GetResourceProperty<Recover>("recover");
    }
}
