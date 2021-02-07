// <copyright file="IIdxContext.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    public interface IIdxContext
    {
        string CodeVerifier { get; }

        string CodeChallenge { get; }

        string CodeChallengeMethod { get; }

        string InteractionHandle { get; }
    }
}
