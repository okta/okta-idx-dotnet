// <copyright file="IAuthenticators.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;
using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent the authenticators object from the IDX response.
    /// </summary>
    public interface IAuthenticators : IResource
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the value property which contains a list of authenticators.
        /// </summary>
        IList<IAuthenticatorValue> Value { get; }
    }
}
