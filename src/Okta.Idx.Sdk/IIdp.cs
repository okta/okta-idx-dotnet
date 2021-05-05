using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public interface IIdp : IResource
    {
        string Id { get; }

        string Name { get; }
    }
}
