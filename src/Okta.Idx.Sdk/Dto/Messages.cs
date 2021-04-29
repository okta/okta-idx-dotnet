// <copyright file="Messages.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class Messages : Resource, IMessages
    {
        /// <inheritdoc/>
        public string Type => GetStringProperty("type");

        /// <inheritdoc/>
        public IList<IMessageItem> MessageItem => GetArrayProperty<IMessageItem>("value");
    }
}
