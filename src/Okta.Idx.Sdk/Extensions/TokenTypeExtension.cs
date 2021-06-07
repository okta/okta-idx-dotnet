// <copyright file="TokenTypeExtension.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk.Extensions
{
    public static class TokenTypeExtension
    {
        public static string ToTokenHintString(this TokenType tokenTypeEnum)
        {
            switch (tokenTypeEnum)
            {
                case TokenType.AccessToken:
                    return "access_token";
                    break;
                case TokenType.RefreshToken:
                    return "refresh_token";
                    break;
                default:
                    return string.Empty;
            }
        }
    }
}
