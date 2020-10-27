using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.IdentityEngine.Sdk
{
    public interface IOktaIdentityEngineClient : IOktaClient
    {
        // TODO: get client id, issuer and scopes from config
        Task<IOktaIdentityEngineResponse> Start(string issuer, string clientId, IList<string> scopes);

        Task<Resource> Interact(string issuer, string clientId, IList<string> scopes);

        Task<IOktaIdentityEngineResponse> Introspect(string interactionHandle, CancellationToken cancellationToken = default);
    }
}
