// <copyright file="IdxSuccessResponse.cs" company="Okta, Inc">
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
    public class IdxSuccessResponse : Resource, IIdxSuccessResponse
    {
        public IList<string> Rel => GetArrayProperty<string>("rel");

        public string Name => GetStringProperty("name");

        public string Method => GetStringProperty("method");

        public string Href => GetStringProperty("href");

        public string Accepts => GetStringProperty("accepts");

        private string GetInteractionCode()
        {
            var successWithInteractionCodeFormValues = this.GetArrayProperty<FormValue>("value");

            var interactionCodeFormValue = successWithInteractionCodeFormValues.FirstOrDefault(x => x.Name == "interaction_code");

            return interactionCodeFormValue.GetProperty<string>("value");
        }

        public async Task<ITokenResponse> ExchangeCodeAsync(IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var client = GetClient();
            var payload = new Dictionary<string, string>();
            payload.Add("interaction_code", GetInteractionCode());
            payload.Add("grant_type", "interaction_code");

            // Add PKCE params
            payload.Add("code_verifier", idxContext.CodeVerifier);
            payload.Add("client_id", client.Configuration.ClientId);

            if (client.Configuration.IsConfidentialClient)
            {
                payload.Add("client_secret", client.Configuration.ClientSecret);
            }

            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", HttpRequestContentBuilder.ContentTypeFormUrlEncoded);

            var request = new HttpRequest
            {
                Uri = Href,
                Payload = payload,
                Headers = headers,
            };

            var httpVerb = (HttpVerb)Enum.Parse(typeof(HttpVerb), Method, true);

            return await client.SendAsync<TokenResponse>(request, httpVerb, cancellationToken).ConfigureAwait(false);
        }
    }
}
