// <copyright file="AuthorizationSettings.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Sdk.Abstractions
{
    public class AuthorizationSettings
    {
        public AuthorizationType AuthorizationType { get; set; } = AuthorizationType.None;

        public string Value { get; set; }

        public static AuthorizationSettings GetDefault()
                => new AuthorizationSettings { AuthorizationType = AuthorizationType.None };
    }
}
