﻿// <copyright file="PasswordRequiredResponse.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Class used to determine if passcode field is present.
    /// </summary>
    public class PasswordRequiredResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the passcode field is present.
        /// </summary>
        public bool IsPasswordRequired { get; set; }

        /// <summary>
        /// Gets the state handle for the IDX context.
        /// </summary>
        public string State { get => Context?.State; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public IIdxContext Context { get; set; }
    }
}
