// <copyright file="IIdxMessages.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent Idx Messages type
    /// </summary>
    public interface IIdxMessages : IResource
    {
        string Type { get; }

        IList<IMessage> Messages { get; }
    }
}
