// <copyright file="FormValue.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class Form : Resource, IForm
    {
        /// <inheritdoc/>
        public bool? Visible => GetBooleanProperty("visible");

        /// <inheritdoc/>
        public bool? Mutable => GetBooleanProperty("mutable");

        /// <inheritdoc/>
        public IList<IField> Fields => GetArrayProperty<IField>("value");
    }
}
