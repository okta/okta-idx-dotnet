// <copyright file="MessageItem.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class MessageItem : Resource, IMessageItem
    {
        /// <inheritdoc/>
        public string Class => GetStringProperty("class");

        /// <inheritdoc/>
        public II18nInformation I18nInformation => GetProperty<II18nInformation>("i18n");

        /// <inheritdoc/>
        public string Message => GetStringProperty("message");
    }
}
