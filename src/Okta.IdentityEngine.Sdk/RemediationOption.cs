using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.IdentityEngine.Sdk
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

        public Task Finalize()
        {
            throw new NotImplementedException();
        }

        public async Task<IOktaIdentityEngineResponse> Proceed(IdentityEngineRequest dataFromUi, CancellationToken cancellationToken =  default)
        {
            var request = new HttpRequest
            {
                Uri = Href,
                Payload = dataFromUi,
            };

            var httpVerb = (HttpVerb)Enum.Parse(typeof(HttpVerb), Method, true);

            return await _client.SendAsync<OktaIdentityEngineResponse>(request, httpVerb, cancellationToken).ConfigureAwait(false);
        }
    }
}
