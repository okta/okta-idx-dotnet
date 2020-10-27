using Microsoft.Extensions.Logging;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.IdentityEngine.Sdk
{
    public class OktaIdentityEngineClient : BaseOktaClient, IOktaIdentityEngineClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OktaIdentityEngineClient"/> class using the specified <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="apiClientConfiguration">
        /// The client configuration. If <c>null</c>, the library will attempt to load
        /// configuration from an <c>okta.yaml</c> file or environment variables.
        /// </param>
        /// <param name="httpClient">The HTTP client to use for requests to the Okta API.</param>
        /// <param name="logger">The logging interface to use, if any.</param>
        public OktaIdentityEngineClient(
            OktaClientConfiguration apiClientConfiguration = null,
            HttpClient httpClient = null,
            ILogger logger = null)
            : base(
                apiClientConfiguration,
                httpClient,
                logger,
                new UserAgentBuilder("okta-identity-engine-dotnet", typeof(OktaIdentityEngineClient).GetTypeInfo().Assembly.GetName().Version),
                new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaIdentityEngineClient"/> class.
        /// </summary>
        /// <param name="dataStore">The <see cref="IDataStore">DataStore</see> to use.</param>
        /// <param name="configuration">The client configuration.</param>
        /// <param name="requestContext">The request context, if any.</param>
        /// <remarks>This overload is used internally to create cheap copies of an existing client.</remarks>
        protected OktaIdentityEngineClient(IDataStore dataStore, OktaClientConfiguration configuration, RequestContext requestContext)
            : base(dataStore, configuration, requestContext)
        {
        }


        public Task<Resource> Interact(string issuer, string clientId, IList<string> scopes)
        {
            throw new NotImplementedException();
        }

        public async Task<IOktaIdentityEngineResponse> Introspect(string interactionHandle, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payload = new IdentityEngineRequest()
            {
                StateHandle = interactionHandle,
            };

            var uri = $"{this.Configuration.OktaDomain}/idp/idx/introspect";
            var request = new HttpRequest
            {
                Uri = uri,
                Payload = payload,
            };

            return await PostAsync<OktaIdentityEngineResponse>(
                request, cancellationToken).ConfigureAwait(false);
        }

        public Task<IOktaIdentityEngineResponse> Start(string issuer, string clientId, IList<string> scopes)
        {
            throw new NotImplementedException();
        }
    }
}
