using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<ITokenResponse> ExchangeCodeAsync(CancellationToken cancellationToken = default)
        {
            var client = GetClient();
            var payload = new Dictionary<string, string>();
            payload.Add("interaction_code", GetInteractionCode());
            payload.Add("grant_type", "interaction_code");

            // TODO: Add PKCE params

            if (client.Configuration.IsConfidentialClient)
            {
                // Create scoped client.
                var requestContext = new RequestContext();
                requestContext.AuthorizationSettings = new AuthorizationSettings()
                {
                    AuthorizationType = AuthorizationType.Basic,
                    Value = AuthorizationSettings.EncodeClientCredentials(client.Configuration.ClientId, client.Configuration.ClientSecret),
                };

                client = (IIdxClient)client.CreateScoped(requestContext);
            }

            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", HttpRequestContentBuilder.CONTENT_TYPE_X_WWW_FORM_URL_ENCODED);

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
