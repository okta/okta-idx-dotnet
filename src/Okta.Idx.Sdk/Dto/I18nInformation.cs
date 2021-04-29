// <copyright file="I18nInformation.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk.Dto
{
    public class I18nInformation : Resource, II18nInformation
    {
        /// <inheritdoc>
        public string Key => GetStringProperty("key");

        /// <inheritdoc>
        public IList<string> Params => GetArrayProperty<string>("params");
    }
}
