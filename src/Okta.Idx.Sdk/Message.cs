// <copyright file="Message.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc/>
    public class Message : Resource, IMessage
    {
        public string Text => GetStringProperty("message");

        public string Class => GetStringProperty("class");
    }
}
