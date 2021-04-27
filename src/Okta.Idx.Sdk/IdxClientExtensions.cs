// <copyright file="IdxClientExtensions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    internal static class IdxClientExtensions
    {
        internal static async Task<IIdxResponse> ContinueWithRemediationOption(this IIdxResponse response, string remediationType, IdxRequestPayload request, CancellationToken cancellationToken = default)
        {
            var remediationOption = response
                    .Remediation
                    .RemediationOptions
                    .FirstOrDefault(x => x.Name == remediationType);

            if (remediationOption == null)
            {
                throw new UnexpectedRemediationException(remediationType, response);
            }

            return await remediationOption
                    .ProceedAsync(request, cancellationToken);
        }

        internal static async Task<IIdxResponse> ContinueWithFirstFoundRemediationOption(this IIdxResponse response, IList<string> remediationTypeNames, IdxRequestPayload request, CancellationToken cancellationToken = default)
        {
            foreach (var remediationTypeName in remediationTypeNames)
            {
                IRemediationOption remediationOption = response
                                                        .Remediation
                                                        .RemediationOptions
                                                        .FirstOrDefault(x => x.Name == remediationTypeName);

                if (remediationOption != null)
                {
                    return await remediationOption
                            .ProceedAsync(request, cancellationToken);
                }
            }

            throw new UnexpectedRemediationException(remediationTypeNames, response);
        }

        internal static bool ContainsRemediationOption(this IIdxResponse response, string remediationType) =>
                    response.Remediation.RemediationOptions.Any(x => x.Name == remediationType);

    }
}
