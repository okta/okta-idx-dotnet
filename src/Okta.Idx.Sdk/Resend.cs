using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    using System.Threading;
    using System.Threading.Tasks;

    using Okta.Sdk.Abstractions;

    public class Resend : Resource, IResend
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

        public async Task<IIdxResponse> ProceedAsync(IdxRequestPayload resendPayload, CancellationToken cancellationToken = default)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/ion+json; okta-version=1.0.0");

            var request = new HttpRequest
                              {
                                  Uri = Href,
                                  Headers = headers,
                                  Payload = resendPayload,
                              };

            var httpVerb = (HttpVerb)Enum.Parse(typeof(HttpVerb), Method, true);

            return await _client.SendAsync<IdxResponse>(request, httpVerb, cancellationToken).ConfigureAwait(false);
        }
    }
}
