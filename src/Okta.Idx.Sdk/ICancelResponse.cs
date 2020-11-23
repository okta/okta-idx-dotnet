using Okta.Sdk.Abstractions;
using System.Collections.Generic;

namespace Okta.Idx.Sdk
{
    public interface ICancelResponse : IResource
    {
        /// <summary>
        /// Gets the ION spec rel member based around the [form structure](https://ionspec.org/#form-structure) rules
        /// </summary>
        IList<string> Rel { get; }

        /// <summary>
        /// Gets the identifier for the remediation option
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the HTTP Method to use for this remediation option.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the Href for the remediation option
        /// </summary>
        string Href { get; }

        /// <summary>
        /// Gets the Accepts Header for this remediation option.
        /// </summary>
        string Accepts { get; }

    }
}