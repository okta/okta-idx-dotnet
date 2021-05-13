// <copyright file="IAuthenticatorEnrollmentData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the authenticator enrollment.
    /// </summary>
    public interface IAuthenticatorEnrollmentData : IResource
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        IAuthenticatorEnrollment Value { get; }
    }
}
