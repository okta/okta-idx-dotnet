// <copyright file="IAuthenticatorEnrollment.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the authenticator enrollment value.
    /// </summary>
    public interface IAuthenticatorEnrollment : IResource
    {
        /// <summary>
        /// Gets the display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the enrollment ID.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        IList<IAuthenticatorMethod> Methods { get; }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        Resource Profile { get; }

        /// <summary>
        /// Gets the recover object.
        /// </summary>
        IRecover Recover { get; }
    }
}
