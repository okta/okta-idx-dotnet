// <copyright file="IdxResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public class IdxResponse : Resource, IIdxResponse
    {
        public string StateHandle => GetStringProperty("stateHandle");

        public string Version => GetStringProperty("version");

        public DateTimeOffset? ExpiresAt => GetDateTimeProperty("expiresAt");

        public string Intent => GetStringProperty("intent");

        public IRemediation Remediation => GetResourceProperty<Remediation>("remediation");

        public bool IsLoginSuccess => this.GetData().ContainsKey("successWithInteractionCode");

        public IIdxSuccessResponse SuccessWithInteractionCode => GetResourceProperty<IdxSuccessResponse>("successWithInteractionCode");

        public async Task<IIdxResponse> CancelAsync(CancellationToken cancellationToken = default)
        {
            var cancelResponse = this.GetResourceProperty<CancelResponse>("cancel");

            var stateHandleFormValue = cancelResponse.GetArrayProperty<FormValue>("value").FirstOrDefault(x => x.Name == "stateHandle");

            var payload = new IdxRequestPayload()
            {
                StateHandle = stateHandleFormValue.GetProperty<string>("value"),
            };

            // TODO: Get accept from Produces.
            var headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/ion+json; okta-version=1.0.0");

            var request = new HttpRequest
            {
                Uri = cancelResponse.Href,
                Payload = payload,
                Headers = headers,
            };

            var httpVerb = (HttpVerb)Enum.Parse(typeof(HttpVerb), cancelResponse.Method, true);

            return await _client.SendAsync<IdxResponse>(request, httpVerb, cancellationToken).ConfigureAwait(false);
        }
    }
}
