using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    public class RemediationOption : Resource, IRemediationOption
    {
        public IList<string> Rel => GetArrayProperty<string>("rel");

        public string Name => GetStringProperty("name");

        public string Method => GetStringProperty("method");

        public string Href => GetStringProperty("href");

        public string Accepts => GetStringProperty("accepts");

        public string RelatesTo => GetStringProperty("relatesTo");

        public IList<IFormValue> Form => GetArrayProperty<IFormValue>("value");

        public async Task<IIdxResponse> ProceedAsync(IdentityEngineRequest dataFromUi, CancellationToken cancellationToken =  default)
        {
            var request = new HttpRequest
            {
                Uri = Href,
                Payload = dataFromUi,
            };

            var httpVerb = (HttpVerb)Enum.Parse(typeof(HttpVerb), Method, true);

            return await _client.SendAsync<IdxResponse>(request, httpVerb, cancellationToken).ConfigureAwait(false);
        }
    }
}
