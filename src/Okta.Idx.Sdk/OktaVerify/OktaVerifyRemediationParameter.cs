// <copyright file="RemediationOptionParameter.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk.OktaVerify
{
    /// <summary>
    /// Represents a parameter to a remediation option.
    /// </summary>
    public class OktaVerifyRemediationParameter : Resource
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => GetStringProperty("name");

        /// <summary>
        /// Gets the label.
        /// </summary>
        public string Label => GetStringProperty("label");

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value => GetStringProperty("value");

        /// <summary>
        /// Gets a value indicating whether this parameter is required.
        /// </summary>
        public bool? IsRequired => GetBooleanProperty("required");
    }
}
