// <copyright file="IIonApiError.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Sdk.Abstractions
{
    /// <summary>
    /// Represents an json+ion formatted error returned by the Okta API.
    /// </summary>
    public interface IIonApiError
    {
        /// <summary>
        /// Gets the <c>version</c> property.
        /// </summary>
        /// <value>The version.</value>
        string Version { get; }

        /// <summary>
        /// Gets the <c>errorSummary</c> property.
        /// </summary>
        /// <value>The error summary.</value>
        string ErrorSummary { get; }
    }
}
