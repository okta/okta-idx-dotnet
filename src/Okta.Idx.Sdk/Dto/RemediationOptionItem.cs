// <copyright file="FormValue.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class RemediationOptionItem : Resource, IRemediationOptionItem
    {
        /// <inheritdoc/>
        public string Name => GetStringProperty("name");

        /// <inheritdoc/>
        public string Type => GetStringProperty("type");

        /// <inheritdoc/>
        public string RelatesTo => GetStringProperty("relatesTo");

        /// <inheritdoc/>
        IForm IRemediationOptionItem.Form => GetProperty<Form>("form");
    }
}
