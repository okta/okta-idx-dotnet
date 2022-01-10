﻿// <copyright file="IAuthenticatorContextualData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents a Web Authn authenticator's contextual data
    /// </summary>
    public interface IAuthenticatorContextualData : IResource
    {
        /// <summary>
        /// Gets the activation data.
        /// </summary>
        IActivationData ActivationData { get; }

        /// <summary>
        /// Gets the challenge data.
        /// </summary>
        IChallengeData ChallengeData { get; }
    }
}