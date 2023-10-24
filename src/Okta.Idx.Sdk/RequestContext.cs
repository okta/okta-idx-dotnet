// <copyright file="RequestContext.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A class to represent the device context information.
    /// </summary>
    public class RequestContext
    {
        private Dictionary<string, string> _headers;

        /// <summary>
        /// Gets or sets the x-device-token header.
        /// </summary>
        /// <remarks>For confidential clients only. The SDK will not send this header if the client is public (no client secret provided). </remarks>
        public string DeviceToken
        {
            get => GetHeader(RequestHeaders.XDeviceToken);
            set => UseHeader(RequestHeaders.XDeviceToken, value);
        }

        /// <summary>
        /// Gets or sets the x-forwarded-for header
        /// </summary>
        /// <remarks>For confidential clients only. The SDK will not send this header if the client is public (no client secret provided). </remarks>
        public string XForwardedFor
        {
            get => GetHeader(RequestHeaders.XForwardedFor);
            set => UseHeader(RequestHeaders.XForwardedFor, value);
        }

        /// <summary>
        /// Gets or sets the x-okta-user-agent-extended header
        /// </summary>
        public string OktaUserAgentExtended
        {
            get => GetHeader(RequestHeaders.XOktaUserAgentExtended);
            set => UseHeader(RequestHeaders.XOktaUserAgentExtended, value);
        }

        /// <summary>
        /// Gets the request headers collection
        /// </summary>
        internal Dictionary<string, string> Headers => _headers;

        /// <summary>
        /// Stores the request header
        /// </summary>
        /// <param name="key">The header name</param>
        /// <param name="value">The header value</param>
        /// <returns>The RequestOptions object</returns>
        private RequestContext UseHeader(string key, string value)
        {
            if (_headers == null)
            {
                _headers = new Dictionary<string, string>();
            }

            _headers[key] = value;
            return this;
        }

        private string GetHeader(string key)
        {
            if (_headers != null && _headers.ContainsKey(key))
            {
                return _headers[key];
            }

            return null;
        }
    }
}
