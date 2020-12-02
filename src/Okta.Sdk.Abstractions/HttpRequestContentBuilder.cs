// <copyright file="HttpRequestContentBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Okta.Sdk.Abstractions
{
    /// <summary>
    /// The request content builder.
    /// </summary>
    public static class HttpRequestContentBuilder
    {
        /// <summary>
        /// application/json
        /// </summary>
        public const string ContentTypeJson = "application/json";

        /// <summary>
        /// "application/x-www-form-urlencoded"
        /// </summary>
        public const string ContentTypeFormUrlEncoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// Get an HttpContent.
        /// </summary>
        /// <param name="contentType"> The request's content type.</param>
        /// <param name="body"> The request's body.</param>
        /// <returns>The request's HttpContent.</returns>
        public static HttpContent GetRequestContent(string contentType = ContentTypeJson, string body = null)
        {
            switch (contentType)
            {
                case ContentTypeJson:
                    return string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, contentType);
                case ContentTypeFormUrlEncoded:
                    return string.IsNullOrEmpty(body) ? null : new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(body));
                default:
                    return string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, contentType);
            }
        }
    }
}
