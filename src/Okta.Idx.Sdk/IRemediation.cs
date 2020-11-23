using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public interface IRemediation : IResource
    {
        string Type { get; }

        IList<IRemediationOption> RemediationOptions { get; }
    }
}
