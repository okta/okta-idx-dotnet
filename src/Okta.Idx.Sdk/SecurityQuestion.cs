// <copyright file="SecurityQuestion.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Idx.Sdk
{
    /// <inheritdoc />
    public class SecurityQuestion : Resource, ISecurityQuestion
    {
        /// <inheritdoc />
        public string QuestionKey => GetStringProperty("questionKey");

        /// <inheritdoc />
        public string Question => GetStringProperty("question");
    }
}
