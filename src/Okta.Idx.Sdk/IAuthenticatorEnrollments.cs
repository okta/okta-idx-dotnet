// <copyright file="IAuthenticatorEnrollments.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the user's authenticator enrollments.
    /// </summary>
    public interface IAuthenticatorEnrollments : IResource
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the authenticators enrollments.
        /// </summary>
        IList<IAuthenticatorEnrollment> Value { get; }
    }
}
