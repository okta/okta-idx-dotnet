﻿// <copyright file="SecurityQuestionAuthenticatorOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Options to answer a security question.
    /// </summary>
    public class SecurityQuestionAuthenticatorOptions
    {
        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Gets or sets the question key.
        /// </summary>
        public string QuestionKey { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        public string Question { get; set; }
    }
}
