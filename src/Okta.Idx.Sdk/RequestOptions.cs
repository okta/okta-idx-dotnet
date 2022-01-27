// <copyright file="RequestOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Request options to be held in this class
    /// </summary>
    public class RequestOptions
    {
        private Dictionary<string, string> _headers;

        /// <summary>
        /// Gets the request headers collection
        /// </summary>
        public Dictionary<string, string> Headers => _headers;

        /// <summary>
        /// Stores the request header
        /// </summary>
        /// <param name="key">The hader name</param>
        /// <param name="value">The header value</param>
        /// <returns>The RequestOptions object</returns>
        public RequestOptions UseHeader(string key, string value)
        {
            if (_headers == null)
            {
                _headers = new Dictionary<string, string>();
            }

            _headers[key] = value;
            return this;
        }
    }
}
