// <copyright file="IAuthenticatorContextualData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets the QrCode.
        /// This is applicable to TOTP authenticators such as Google Authenticator.
        /// </summary>
        IQrCode QrCode { get; }

        /// <summary>
        /// Gets the Shared Secret.
        /// This is the value encoded into the QrCode and can be used to compute a TOTP.
        /// </summary>
        string SharedSecret { get; }

        /// <summary>
        /// Gets the question keys, related to security questions.
        /// </summary>
        IList<string> QuestionKeys { get; }

        /// <summary>
        /// Gets the security questions.
        /// </summary>
        IList<SecurityQuestion> Questions { get; } 
    }
}
