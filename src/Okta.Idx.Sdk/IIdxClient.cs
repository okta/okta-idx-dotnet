using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    public interface IIdxClient : IOktaClient
    {
        /// <summary>
        /// Call the Okta Identity Engine introspect endpoint to get remediation steps. if Interaction Handle is
        /// null, the SDK MUST make an api call to the interact endpoint to get the initial inteactionHandle.
        /// </summary>
        /// <param name="interactionHandle">The interaction handle that was returned by the `interact()` call</param>
        /// <returns>The OktaIdentityEngineResponse.</returns>
        Task<IIdxResponse> StartAsync(string interactionHandle = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
