// <copyright file="Remediation.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    public class Remediation : Resource, IRemediation
    {
        public string Type => GetStringProperty("type");

        public IList<IRemediationOption> RemediationOptions => GetArrayProperty<IRemediationOption>("value");
    }
}
