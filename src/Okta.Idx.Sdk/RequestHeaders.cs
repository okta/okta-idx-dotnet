// <copyright file="RequestHeaders.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
/// <summary>
/// Pre-defined request headers
/// </summary>
    public static class RequestHeaders
    {
        public const string ContentType = "Content-Type";
        public const string UserAgent = "User-Agent";
        public const string XOktaUserAgentExtended = "X-Okta-User-Agent-Extended";
        public const string XForwardedFor = "X-Forwarded-For";
        public const string XDeviceToken = "X-Device-Token";
    }
}
