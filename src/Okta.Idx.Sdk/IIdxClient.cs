using Okta.Idx.Sdk.Configuration;
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
        /// Calls the Idx introspect endpoint to get remediation steps. if Interaction Handle is
        /// null, the SDK MUST make an API call to the interact endpoint to get the initial inteactionHandle.
        /// </summary>
        /// <param name="interactionHandle">The interaction handle that was returned by the `interact()` call</param>
        /// <returns>The IdxResponse.</returns>
        Task<IIdxResponse> IntrospectAsync(string interactionHandle = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Calls the Idx interact endpoint to get an interaction handle.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The Interaction Response.</returns>
        Task<IInteractionHandleResponse> InteractAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the client configuration.
        /// </summary>
        IdxConfiguration Configuration { get; }
        
    }
}
