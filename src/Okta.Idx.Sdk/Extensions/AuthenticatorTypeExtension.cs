// <copyright file="AuthenticatorTypeExtension.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    public static class AuthenticatorTypeExtension
    {
        public static string ToIdxKeyString(this AuthenticatorType authenticatorTypeEnum)
        {
            switch (authenticatorTypeEnum)
            {
                case AuthenticatorType.Email:
                    return "okta_email";
                    break;
                case AuthenticatorType.Sms:
                    return "okta_sms";
                    break;
                case AuthenticatorType.Password:
                    return "okta_password";
                    break;
                default:
                    return string.Empty;
            }
        }
    }
}
