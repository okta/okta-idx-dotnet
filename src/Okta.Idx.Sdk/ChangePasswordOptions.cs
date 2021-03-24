// <copyright file="ChangePasswordOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A class to represent the options required to change a password.
    /// </summary>
    public class ChangePasswordOptions
    {
        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
