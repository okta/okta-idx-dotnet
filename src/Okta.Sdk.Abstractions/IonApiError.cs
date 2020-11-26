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
