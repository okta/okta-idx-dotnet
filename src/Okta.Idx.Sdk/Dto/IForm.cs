// <copyright file="IForm.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IForm : IResource
    {
        /// <summary>
        /// Gets the value that indicates if this form value should be visible in a UI
        /// </summary>
        bool? Visible { get; }

        /// <summary>
        /// Gets the value that indicates if this form value should be mutable in a UI. MAY relate to the form fields `diabled` property
        /// </summary>
        bool? Mutable { get; }

        /// <summary>
        /// Gets form's fields list
        /// </summary>
        IList<IField> Fields { get; }
    }
}