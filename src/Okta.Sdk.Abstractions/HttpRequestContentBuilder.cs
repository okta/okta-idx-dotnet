using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Okta.Sdk.Abstractions
{
    public static class HttpRequestContentBuilder
    {
        public const string CONTENT_TYPE_JSON = "application/json";
        public const string CONTENT_TYPE_X_WWW_FORM_URL_ENCODED = "application/x-www-form-urlencoded";
        // TODO: Add ION

        /// <summary>
        /// Get an HttpContent.
        /// </summary>
        /// <returns>The request's HttpContent.</returns>
        public static HttpContent GetRequestContent(string contentType = CONTENT_TYPE_JSON, string body = null)
        {
            switch (contentType)
            {
                case CONTENT_TYPE_JSON:
                    return string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, contentType);
                case CONTENT_TYPE_X_WWW_FORM_URL_ENCODED:
                    return string.IsNullOrEmpty(body) ? null : new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(body));
                default:
                    return string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, contentType);
            }
        }

    }
}
