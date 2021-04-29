// <copyright file="IMessages.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IMessages : IResource
    {
        string Type { get; }

        IList<IMessageItem> MessageItem { get; }
    }
}
