// <copyright file="ChallengeData.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <inheritdoc cref="IChallengeData"/>
    public class ChallengeData : Resource, IChallengeData
    {
        /// <inheritdoc/>
        public string Challenge => GetStringProperty("challenge");

        /// <inheritdoc/>
        public string UserVerification => GetStringProperty("userVerification");

        /// <inheritdoc/>
        public Resource Extensions => GetResourceProperty<Resource>("extensions");
    }
}
