using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Represents information about an identity provider used to render links.
    /// </summary>
    public class IdpOption
    {
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the interaction handle.
        /// </summary>
        public string InteractionHandle { get; set; }

        /// <summary>
        /// Gets or sets the HREF.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}
