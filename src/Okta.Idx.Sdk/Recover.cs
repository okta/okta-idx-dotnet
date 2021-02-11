// <copyright file="Recover.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A class to represent the <code>entity</code>.
    /// </summary>
    public class Recover : Resource, IRecover
    {
        /// <inheritdoc/>
        public IList<string> Rel => GetArrayProperty<string>("rel");

        /// <inheritdoc/>
        public string Name => GetStringProperty("name");

        /// <inheritdoc/>
        public string Method => GetStringProperty("method");

        /// <inheritdoc/>
        public string Href => GetStringProperty("href");

        /// <inheritdoc/>
        public string Accepts => GetStringProperty("accepts");

        /// <inheritdoc/>
        public IList<IFormValue> Form => GetArrayProperty<IFormValue>("value");

        /// <inheritdoc/>
        public async Task<IIdxResponse> ProceedAsync(IdxRequestPayload recoveryPayload, CancellationToken cancellationToken = default)
        {
            // TODO: Get accept from Produces.
            var headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/ion+json; okta-version=1.0.0");

            var request = new HttpRequest
            {
                Uri = Href,
                Payload = recoveryPayload,
                Headers = headers,
            };

            var httpVerb = (HttpVerb)Enum.Parse(typeof(HttpVerb), Method, true);

            return await _client.SendAsync<IdxResponse>(request, httpVerb, cancellationToken).ConfigureAwait(false);
        }
    }
}
