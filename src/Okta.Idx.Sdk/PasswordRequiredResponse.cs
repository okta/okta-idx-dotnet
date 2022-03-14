using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Class used to determine if passcode field is present.
    /// </summary>
    public class PasswordRequiredResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the passcode field is present.
        /// </summary>
        public bool IsPasswordRequired { get; set; }

        /// <summary>
        /// Gets the state handle for the IDX context.
        /// </summary>
        public string State { get => Context?.State; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public IIdxContext Context { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public IIdxResponse Response { get; set; }
    }
}
