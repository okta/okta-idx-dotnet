// <copyright file="IIonForm.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IIonForm : IResource
    {
        string Accepts { get; set; }

        string Href { get; set; }

        string Method { get; set; }

        string Name { get; set; }

        string Produces { get; set; }

        int? Refresh { get; set; }

        IList<string> Rel { get; set; }

        IList<string> RelatesTo { get; set; }

        IList<IIonField> Value { get; }
    }
}
