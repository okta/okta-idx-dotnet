// <copyright file="IAuthenticator.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent an authenticator.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Gets or sets the authenticator ID.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the enrollment ID if available.
        /// </summary>
        string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the authenticator name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        string Profile { get; set; }

        /// <summary>
        /// Gets or sets the authenticator method types if applicable.
        /// </summary>
        IList<AuthenticatorMethodType> MethodTypes { get; set; }
    }
}