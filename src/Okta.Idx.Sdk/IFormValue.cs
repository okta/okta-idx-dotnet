// <copyright file="IFormValue.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    public interface IFormValue : IResource
    {
        /// <summary>
        /// Gets the name of the form item that can be used in a UI. This relates to the name that is used for the body of the request for the RemediationOption.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a user friendly name that could be used for a UI
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the type of value that is expected
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the value that indicates if this form value should be visible in a UI
        /// </summary>
        bool? Visible { get; }

        /// <summary>
        /// Gets the value that indicates if this form value should be mutable in a UI. MAY relate to the form fields `diabled` property
        /// </summary>
        bool? Mutable { get; }

        /// <summary>
        /// Gets the value that indicates if this form value should be required.
        /// </summary>
        bool? Required { get; }

        /// <summary>
        /// Returns an object that is populated from the json path.
        /// Example: `$.authenticatorEnrollments.value[0]` would relate to the jsonPath `OktaIdentityEngine->raw()->authenticatorEnrollments->value[0]`
        /// </summary>
        string RelatesTo { get; }

        /// <summary>
        /// In the case of a nested object, this will give you the items in the nest. Example: if `$this->type == "object"`, form() will return an array of FormValue objects
        /// </summary>
        IFormValue Form { get; }

        /// <summary>
        /// Gets the value that indicates if this form value should be secret
        /// </summary>
        bool? Secret { get; }

        /// <summary>
        /// Gets a list of options that is described as an array of formValue. Will be null if $this->type == "object" but `options` key does not exist
        /// </summary>
        IList<IFormValue> Options { get; }
    }
}
