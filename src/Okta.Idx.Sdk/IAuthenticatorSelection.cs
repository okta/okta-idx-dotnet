// <copyright file="IAuthenticatorSelection.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents the authenticator selections options of a Web Authn authenticator
    /// </summary>
    public interface IAuthenticatorSelection : IResource
    {
        /// <summary>
        /// Gets the <c>RequireResidentKey</c> value.
        /// </summary>
        bool? RequireResidentKey { get; }

        /// <summary>
        /// Gets the user verification.
        /// </summary>
        string UserVerification { get; }
    }
}
