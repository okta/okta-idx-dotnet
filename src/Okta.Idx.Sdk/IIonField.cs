// <copyright file="IIonField.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IIonField : IResource
    {
        IIonForm Form { get; set; }

        string Label { get; set; }

        bool? Mutable { get; set; }

        string Name { get; set; }

        bool? Required { get; set; }

        bool? Secret { get; set; }

        string Type { get; set; }

        Resource Value { get; set; }

        bool? Visible { get; set; }
    }
}
