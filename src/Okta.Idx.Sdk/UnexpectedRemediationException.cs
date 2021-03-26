// <copyright file="UnexpectedRemediationException.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An exception to be thrown when an unexpected remediation has been received.
    /// </summary>
    public class UnexpectedRemediationException : OktaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedRemediationException"/> class.
        /// </summary>
        /// <param name="expectedRemediationName">The expected remediation name.</param>
        /// <param name="receivedResponse">The received response.</param>
        public UnexpectedRemediationException(string expectedRemediationName, IIdxResponse receivedResponse)
            : base(GetDetailedErrorMessage(expectedRemediationName, receivedResponse))
        {
        }

        private static string GetDetailedErrorMessage(string expectedRemediationName, IIdxResponse receivedResponse)
        {
            var remediationNamesList = receivedResponse?.Remediation?.RemediationOptions?.Select(x => x.Name).ToList() ?? new List<string>();

            return $@"Unexpected remediation step: Expected '{expectedRemediationName}' but received ['{string.Join(",", remediationNamesList)}'].
                    Verify that your policies are configured as expected.";
        }
    }
}
