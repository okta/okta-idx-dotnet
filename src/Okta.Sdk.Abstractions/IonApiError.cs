// <copyright file="IonApiError.cs" company="Okta, Inc">
// Copyright (c) 2018 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Sdk.Abstractions
{
    /// <inheritdoc/>
    public sealed class IonApiError : BaseResource, IIonApiError
    {
        /// <inheritdoc/>
        public string Version => GetStringProperty("version");

        /// <inheritdoc/>
        public string ErrorSummary => GetErrorSummary();

        private string GetErrorSummary()
        {
            var sbErrorSumary = new StringBuilder();
            var messageObj = this.GetProperty<BaseResource>("messages");

            if (messageObj != null)
            {
                var messages = messageObj.GetArrayProperty<BaseResource>("value");

                if (messages != null)
                {
                    foreach (var message in messages)
                    {
                        sbErrorSumary.AppendLine(message.GetProperty<string>("message"));
                    }
                }
            }

            return sbErrorSumary.ToString();
        }
    }
}
