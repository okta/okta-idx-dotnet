// <copyright file="IActivationData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IActivationData : IResource
    {
        string Attestation { get; }

        IAuthenticatorSelection AuthenticatorSelection { get; }

        string Challenge { get; }

        IList<IPublicKeyCredParam> PublicKeyCredParams { get; }

        IUser User { get; }
    }
}
