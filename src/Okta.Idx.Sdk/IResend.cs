// <copyright file="IResend.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
   /// <summary>
   /// The IResend Interface
   /// </summary>
    public interface IResend : IResource
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
        /// Gets the HTTP Method to use for the recovery.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the Href for the recovery
        /// </summary>
        string Href { get; }

        /// <summary>
        /// Gets the Accepts Header for the recovery
        /// </summary>
        string Accepts { get; }

        /// <summary>
        /// Gets all form values. This is generated from `$this->value`. Each item in `$this->value` MUST be mapped to a `FormValue` object.
        /// </summary>
        IList<IFormValue> Form { get; }

        /// <summary>
        /// Allows you to proceed with the recovery.
        /// </summary>
        /// <param name="resendPayload">The payload</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>An IdxResponse.</returns>
        Task<IIdxResponse> ProceedAsync(IdxRequestPayload resendPayload, CancellationToken cancellationToken = default);
    }
}
