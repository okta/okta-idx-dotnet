// <copyright file="IActivationData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// The activation data of a WebAuthn authenticator.
    /// </summary>
    public interface IActivationData : IResource
    {
        /// <summary>
        /// Gets the attestation.
        /// </summary>
        string Attestation { get; }

        /// <summary>
        /// Gets the authenticator selection.
        /// </summary>
        IAuthenticatorSelection AuthenticatorSelection { get; }

        /// <summary>
        /// Gets the challenge.
        /// </summary>
        string Challenge { get; }

        /// <summary>
        /// Gets the public key credential params
        /// </summary>
        IList<IPublicKeyCredParam> PublicKeyCredParams { get; }

        /// <summary>
        /// Gets the user info.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// Gets the U2F Params.
        /// </summary>
        IU2fParams U2fParams { get; }
    }
}
