using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.OktaVerify
{
    /// <summary>
    /// Class that contains information relevant to client side polling.
    /// </summary>
    public class OktaVerifyPollResponse
    {
        /// <summary>
        /// Gets or sets the duration to wait until the next poll request, in milliseconds.
        /// </summary>
        public int? Refresh { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to continue polling.
        /// </summary>
        public bool ContinuePolling { get; set; }

        /// <summary>
        /// Gets or sets a path value indicating where to direct the client when polling is complete.
        /// </summary>
        public string Next { get; set; }
    }
}
