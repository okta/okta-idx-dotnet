// <copyright file="IdxClientExtensions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    internal static class IdxClientExtensions
    {
        internal static async Task<PollResponse> PollOnceAsync(this IRemediationOption remediationOption, string stateHandle)
        {
            if (remediationOption.Name != RemediationType.EnrollPoll)
            {
                throw new ArgumentException(string.Format("Expected remediation option of type '{0}', the specified remediation option is of type {1}", RemediationType.EnrollPoll, remediationOption.Name));
            }

            IdxRequestPayload requestPayload = new IdxRequestPayload
            {
                StateHandle = stateHandle,
            };

            var idxResponse = await remediationOption.ProceedAsync(requestPayload);
            bool continuePolling = idxResponse.ContainsRemediationOption(RemediationType.EnrollPoll, out IRemediationOption enrollPollRemediationOption);

            return new PollResponse
            {
                Refresh = enrollPollRemediationOption?.Refresh,
                ContinuePolling = continuePolling,
            };
        }

        internal static async Task<IIdxResponse> ProceedWithRemediationOptionAsync(this IIdxResponse response, string remediationType, IdxRequestPayload request, CancellationToken cancellationToken = default)
        {
            var remediationOption = response.FindRemediationOption(remediationType, true);

            return await remediationOption
                    .ProceedAsync(request, cancellationToken);
        }

        internal static bool ContainsRemediationOption(this IIdxResponse response, string remediationType) =>
                    response.Remediation?.RemediationOptions?.Any(x => x.Name == remediationType) ?? false;

        internal static bool ContainsRemediationOption(this IIdxResponse response, string remediationType, out IRemediationOption remediationOption)
        {
            remediationOption = response.Remediation?.RemediationOptions?.Where(x => x.Name == remediationType)?.FirstOrDefault();
            return remediationOption != null;
        }

        internal static IRemediationOption FindRemediationOption(this IIdxResponse response, string remediationType, bool throwIfNotFound = false)
        {
            var option = response
                .Remediation?
                .RemediationOptions?
                .FirstOrDefault(x => x.Name == remediationType);

            if (option == null && throwIfNotFound)
            {
                throw new UnexpectedRemediationException(remediationType, response);
            }

            return option;
        }

        internal static void AssertNotInTerminalState(this IIdxResponse response)
        {
            if (response?.IdxMessages?.Messages?.Any() ?? false)
            {
                throw new TerminalStateException(response.IdxMessages);
            }
        }
    }
}
