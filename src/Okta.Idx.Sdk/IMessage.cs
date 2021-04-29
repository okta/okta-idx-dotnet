// <copyright file="IMessage.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// An interface to represent a single message in message list.
    /// </summary>
    public interface IMessage : IResource
    {
        /// <summary>
        /// Gets message text
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the class of the message
        /// </summary>
        string Class { get; }

        /// <summary>
        /// Gets internationalization info
        /// </summary>
        II18nInformation I18nInformation { get; }
    }
}
