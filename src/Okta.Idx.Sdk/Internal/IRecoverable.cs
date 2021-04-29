// <copyright file="IRecoverable.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk.Internal
{
    /// <summary>
    /// An interface to represent entities that have a recover property.
    /// </summary>
    internal interface IRecoverable : IResource
    {
        /// <summary>
        /// Gets the recover object.
        /// </summary>
        IRecover Recover { get; }
    }
}
