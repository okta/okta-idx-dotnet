// <copyright file="IIdxI18n.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent an idx i18n type
    /// </summary>
    public interface IIdxI18n
    {
        string Key { get; }

        IList<string> Params { get; }
    }
}
