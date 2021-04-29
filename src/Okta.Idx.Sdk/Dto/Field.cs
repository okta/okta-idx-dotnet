// <copyright file="Field.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class Field : Resource, IField
    {
        /// <inheritdoc/>
        public string Name => GetStringProperty("name");

        /// <inheritdoc/>
        public string Label => GetStringProperty("label");

        /// <inheritdoc/>
        public bool? Visible => GetBooleanProperty("visible");

        /// <inheritdoc/>
        public bool? Mutable => GetBooleanProperty("mutable");

        /// <inheritdoc/>
        public bool? Required => GetBooleanProperty("required");

        /// <inheritdoc/>
        public bool? Secret => GetBooleanProperty("secret");

        /// <inheritdoc/>
        public IList<IFormValue> Options => GetArrayProperty<IFormValue>("options");

        /// <inheritdoc/>
        public IMessages Messages => GetProperty<Messages>("messages");
    }
}
