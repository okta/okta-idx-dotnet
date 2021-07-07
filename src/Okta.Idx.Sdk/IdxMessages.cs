﻿// <copyright file="IdxMessages.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Idx Messages type
    /// </summary>
    public class IdxMessages : Resource, IIdxMessages
    {
        public string Type => GetStringProperty("type");

        public IList<IMessage> Messages => GetArrayProperty<IMessage>("value");
    }
}
