// <copyright file="OktaIonApiException.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Sdk.Abstractions
{
    /// <summary>
    /// An exception wrapping a json+ion formatted error returned by the Okta API.
    /// </summary>
    public class OktaIonApiException : OktaException
    {
        private readonly IonApiError _error;

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaIonApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="error">The error data.</param>
        public OktaIonApiException(int statusCode, IonApiError error)
            : base(message: $"({statusCode}):({error.ErrorSummary})")
        {
            _error = error;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the error object returned by the Okta API.
        /// </summary>
        /// <value>
        /// The error object returned by the Okta API.
        /// </value>
        public IonApiError Error => _error;
    }
}
