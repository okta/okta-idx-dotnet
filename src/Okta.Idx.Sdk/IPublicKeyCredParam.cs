// <copyright file="IPublicKeyCredParam.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents the public key params of a Web Authn authenticator.
    /// </summary>
    public interface IPublicKeyCredParam : IResource
    {
        /// <summary>
        /// Gets the algorithm
        /// </summary>
        string Alg { get; }

        /// <summary>
        /// Gets the type
        /// </summary>
        string Type { get; }
    }
}
