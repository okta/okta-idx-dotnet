// <copyright file="PollResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Class that contains information relevant to client side polling.
    /// </summary>
    public class PollResponse : AuthenticationResponse
    {
        /// <summary>
        /// Gets or sets the duration to wait until the next poll request, in milliseconds.
        /// </summary>
        public int? Refresh { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to continue polling.
        /// </summary>
        public bool ContinuePolling { get; set; }
    }
}
