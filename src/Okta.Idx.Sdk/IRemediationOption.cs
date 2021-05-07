// <copyright file="IRemediationOption.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the remediation option entity.
    /// </summary>
    public interface IRemediationOption : IResource
    {
        /// <summary>
        /// Gets the ION spec rel member based around the <see cref="https://ionspec.org/#form-structure">form structure</see> rules
        /// </summary>
        IList<string> Rel { get; }

        /// <summary>
        /// Gets the identifier for the remediation option
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type for the remediation option
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the HTTP Method to use for this remediation option.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the Href for the remediation option
        /// </summary>
        string Href { get; }

        /// <summary>
        /// Gets the Idp
        /// </summary>
        IIdp Idp { get; }

        /// <summary>
        /// Gets the Accepts Header for this remediation option.
        /// </summary>
        string Accepts { get; }

        /// <summary>
        /// Gets an object that is populated from the json path.
        /// Example: `$.authenticatorEnrollments.value[0]` would relate to the jsonPath `OktaIdentityEngine->raw()->authenticatorEnrollments->value[0]`
        /// </summary>
        string RelatesTo { get; }

        /// <summary>
        /// Gets all form values. This is generated from `$this->value`. Each item in `$this->value` MUST be mapped to a `FormValue` object.
        /// </summary>
        IList<IFormValue> Form { get; }

        /// <summary>
        /// Allows you to continue the remediation with this option.
        /// </summary>
        /// <param name="dataFromUi">The data returned from the enduser</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>An IdxResponse.</returns>
        Task<IIdxResponse> ProceedAsync(IdxRequestPayload dataFromUi, CancellationToken cancellationToken = default);
    }
}
