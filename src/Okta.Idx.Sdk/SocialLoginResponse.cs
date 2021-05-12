// <copyright file="IdxState.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Newtonsoft.Json;
using Okta.Idx.Sdk.Configuration;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A descriptor for the current state of an Idx social login interaction.
    /// </summary>
    public class SocialLoginResponse
    {
        public IIdxContext Context { get; set; }

        public IdpOption[] IdpOptions { get; set; }

        public IdxConfiguration Configuration { get; set; }
    }
}
