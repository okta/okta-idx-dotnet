// <copyright file="IIdxSuccessResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IIdxSuccessResponse : IResource
    {
        /// <summary>
        /// Gets the ION spec rel member based around the [form structure](https://ionspec.org/#form-structure) rules
        /// </summary>
        IList<string> Rel { get; }

        /// <summary>
        /// Gets the identifier for the remediation option
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the HTTP Method to use for this remediation option.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the Href for the remediation option
        /// </summary>
        string Href { get; }

        /// <summary>
        /// Gets the Accepts Header for this remediation option.
        /// </summary>
        string Accepts { get; }

        /// <summary>
        /// Exchange an interaction code for tokens.
        /// </summary>
        /// <param name="idxContext">The IDX context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The tokens' reponse.</returns>
        Task<ITokenResponse> ExchangeCodeAsync(IIdxContext idxContext, CancellationToken cancellationToken = default);
    }
}
