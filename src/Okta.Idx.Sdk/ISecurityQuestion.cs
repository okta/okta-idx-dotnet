using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public interface ISecurityQuestion : IResource
    {
        string QuestionKey { get; }
        string Question { get; }
    }
}
