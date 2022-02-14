﻿// <copyright file="PublicKeyCredParam.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc cref="IPublicKeyCredParam"/>
    public class PublicKeyCredParam : Resource, IPublicKeyCredParam
    {
        /// <inheritdoc/>
        public string Alg => GetStringProperty("alg");

        /// <inheritdoc/>
        public string Type => GetStringProperty("type");
    }
}
