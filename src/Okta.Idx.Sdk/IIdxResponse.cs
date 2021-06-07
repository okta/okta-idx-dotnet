// <copyright file="IIdxResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the IDX response.
    /// </summary>
    public interface IIdxResponse : IResource
    {
        /// <summary>
        /// Gets the stateHandle is used for all calls for the flow.
        /// </summary>
        string StateHandle { get; }

        /// <summary>
        /// Gets the version that needs to be used in the headers
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the value when the current remediation flow expires
        /// </summary>
        DateTimeOffset? ExpiresAt { get; }

        /// <summary>
        /// Gets the intent of the Okta Identity Engine flow
        /// </summary>
        string Intent { get; }

        /// <summary>
        /// Gets the current remediation object. MAY be null if there are no further remediation steps necessary
        /// </summary>
        IRemediation Remediation { get; }

        /// <summary>
        /// Gets a success response object after `loginSuccess()` returns true.
        /// </summary>
        IIdxSuccessResponse SuccessWithInteractionCode { get;  }

        /// <summary>
        /// The method to call when you want to cancel the Okta Identity Engine flow. This will return an IdxResponse
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An IdxResponse.</returns>
        Task<IIdxResponse> CancelAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current authenticator
        /// </summary>
        IAuthenticatorEnrollmentData CurrentAuthenticator { get; }

        /// <summary>
        /// Gets the current authenticator enrollment
        /// </summary>
        IAuthenticatorEnrollmentData CurrentAuthenticatorEnrollment { get; }

        /// <summary>
        /// Gets the authenticators
        /// </summary>
        IAuthenticators Authenticators { get;  }

        /// <summary>
        /// Gets the user's authenticator enrollments
        /// </summary>
        IAuthenticatorEnrollments AuthenticatorEnrollments { get; }

        /// <summary>
        /// Gets a value indicating whether login was successful
        /// </summary>
        bool IsLoginSuccess { get; }

        /// <summary>
        /// Gets messages from the server
        /// </summary>
        IIdxMessages IdxMessages { get; }
    }
}
