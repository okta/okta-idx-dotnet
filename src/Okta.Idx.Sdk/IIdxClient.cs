// <copyright file="IIdxClient.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A client to interact with the IDX API.
    /// </summary>
    public interface IIdxClient : IOktaClient
    {
        /// <summary>
        /// Calls the Idx introspect endpoint to get remediation steps.
        /// </summary>
        /// <param name="idxContext">The IDX context that was returned by the `interact()` call</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The IdxResponse.</returns>
        Task<IIdxResponse> IntrospectAsync(IIdxContext idxContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Calls the Idx interact endpoint to get an IDX context.
        /// </summary>
        /// <param name="state">Optional value to use as the state argument when initiating the authentication flow. This is used to provide contextual information to survive redirects.</param>
        /// <param name="cancellationToken">The cancellation token. Optional.</param>
        /// <returns>The IDX context.</returns>
        Task<IIdxContext> InteractAsync(string state = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the client configuration.
        /// </summary>
        IdxConfiguration Configuration { get; }
    }
}
