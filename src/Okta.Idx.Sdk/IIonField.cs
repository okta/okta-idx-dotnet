using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public interface IIonField : IResource
    {
        IIonForm Form { get; set; }

        string Label { get; set; }

        bool? Mutable { get; set; }

        string Name { get; set; }

        bool? Required { get; set; }

        bool? Secret { get; set; }

        string Type { get; set; }

        Resource Value { get; set; }

        bool? Visible { get; set; }
    }
}
