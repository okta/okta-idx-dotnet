// <copyright file="RedeemInteractionCodeException.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents an exception that may occur on an attempt to redeem an interaction code.
    /// </summary>
    public class RedeemInteractionCodeException : OktaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedeemInteractionCodeException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public RedeemInteractionCodeException(Exception innerException)
            : base("Exception occurred redeeming interaction code.", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedeemInteractionCodeException"/> class.
        /// </summary>
        /// <param name="apiResponse">The API resposne.</param>
        /// <param name="innerException">The inner exception.</param>
        public RedeemInteractionCodeException(string apiResponse, Exception innerException = null)
            : base("Exception occurred redeeming interaction code.", innerException)
        {
            this.ApiResponse = apiResponse;
        }

        /// <summary>
        /// Gets or sets the API response.
        /// </summary>
        public string ApiResponse { get; set; }
    }
}
