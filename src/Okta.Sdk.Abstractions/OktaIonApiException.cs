using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Sdk.Abstractions
{
    public class OktaIonApiException : OktaException
    {
        private readonly IonApiError _error;
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
