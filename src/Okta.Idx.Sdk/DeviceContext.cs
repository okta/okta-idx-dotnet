// <copyright file="DeviceContext.cs" company="Okta, Inc">
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
    public class DeviceContext
    {
        private Dictionary<string, string> _headers;

        /// <summary>
        /// Gets the request headers collection
        /// </summary>
        public Dictionary<string, string> Headers => _headers;

        /// <summary>
        /// Stores the request header
        /// </summary>
        /// <param name="key">The header name</param>
        /// <param name="value">The header value</param>
        /// <returns>The RequestOptions object</returns>
        public DeviceContext UseHeader(string key, string value)
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
