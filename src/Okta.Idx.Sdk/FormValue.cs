// <copyright file="FormValue.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class FormValue : Resource, IFormValue
    {
        /// <inheritdoc/>
        public string Name => GetStringProperty("name");

        /// <inheritdoc/>
        public string Label => GetStringProperty("label");

        /// <inheritdoc/>
        public string Type => GetStringProperty("type");

        /// <inheritdoc/>
        public bool? Visible => GetBooleanProperty("visible");

        /// <inheritdoc/>
        public bool? Mutable => GetBooleanProperty("mutable");

        /// <inheritdoc/>
        public bool? Required => GetBooleanProperty("required");

        /// <inheritdoc/>
        public string RelatesTo => GetStringProperty("relatesTo");

        /// <inheritdoc/>
        public bool? Secret => GetBooleanProperty("secret");

        /// <inheritdoc/>
        public IFormValue Form => GetResourceProperty<FormValue>("form");

        /// <inheritdoc/>
        public IList<IFormValue> Options => GetArrayProperty<IFormValue>("options");
    }
}
