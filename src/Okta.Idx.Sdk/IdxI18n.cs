// <copyright file="IdxI18n.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Idx i18n type
    /// </summary>
    public class IdxI18n : Resource, IIdxI18n
    {
        public string Key => GetStringProperty("key");

        public IList<string> Params => GetArrayProperty<string>("params");
    }
}