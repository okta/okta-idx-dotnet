// <copyright file="IdxClientExtensions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    internal static class IdxClientExtensions
    {
        internal static async Task<IIdxResponse> ProceedWithRemediationOptionAsync(this IIdxResponse response, string remediationType, IdxRequestPayload request, CancellationToken cancellationToken = default)
        {
            var remediationOption = response.FindRemediationOption(remediationType, true);

            return await remediationOption
                    .ProceedAsync(request, cancellationToken);
        }

        internal static async Task<IIdxResponse> ProceedWithRemediationOptionAsync(this IRemediationOption remediationOption, IdxRequestPayload request, CancellationToken cancellationToken = default)
        {
            if (remediationOption == null)
            {
                throw new System.ArgumentNullException(nameof(remediationOption));
            }

            return await remediationOption
                    .ProceedAsync(request, cancellationToken);
        }

        internal static bool ContainsRemediationOption(this IIdxResponse response, string remediationType) =>
                    response.Remediation.RemediationOptions.Any(x => x.Name == remediationType);

        internal static IRemediationOption FindRemediationOption(this IIdxResponse response, string remediationType, bool throwIfNotFound = false)
        {
            var option = response
                .Remediation
                .RemediationOptions
                .FirstOrDefault(x => x.Name == remediationType);

            if (option == null && throwIfNotFound)
            {
                throw new UnexpectedRemediationException(remediationType, response);
            }

            return option;
        }
    }
}
