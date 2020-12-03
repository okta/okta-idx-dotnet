// <copyright file="TokenResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    public class TokenResponse : Resource, ITokenResponse
    {
        public string TokenType => GetStringProperty("token_type");

        public int? ExpiresIn => GetProperty<int?>("expires_in");

        public string AccessToken => GetStringProperty("access_token");

        public string RefreshToken => GetStringProperty("refresh_token");

        public string IdToken => GetStringProperty("id_token");

        public string Scope => GetStringProperty("scope");
    }
}
