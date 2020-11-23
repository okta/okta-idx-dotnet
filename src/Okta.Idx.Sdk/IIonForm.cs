using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public interface IIonForm : IResource
    {
        string Accepts { get; set; }

        string Href { get; set; }

        string Method { get; set; }

        string Name { get; set; }

        string Produces { get; set; }

        int? Refresh { get; set; }

        IList<string> Rel { get; set; }

        IList<string> RelatesTo { get; set; }

        IList<IIonField> Value { get; }
    }
}
