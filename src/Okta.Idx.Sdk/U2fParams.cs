﻿// <copyright file="U2fParams.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// The U2F Params.
    /// </summary>
    public class U2fParams : Resource, IU2fParams
    {
        /// <summary>
        /// Gets or sets the application ID
        /// </summary>
        public string ApplicationId => GetProperty<string>("appid");
    }
}
