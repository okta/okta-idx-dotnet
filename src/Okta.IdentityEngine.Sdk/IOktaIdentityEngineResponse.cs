﻿using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Okta.IdentityEngine.Sdk
{
    public interface IOktaIdentityEngineResponse : IResource
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
        /// The method to call when you want to cancel the Okta Identity Engine flow. This will return an OktaIdentityEngineResponse
        /// </summary>
        /// <returns>An OktaIdentityEngineResponse.</returns>
        Task<IOktaIdentityEngineResponse> Cancel();

        
    }
}
