using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class EnrollPollOptions
    {
        public IRemediationOption RemediationOption { get; set; }
        public string StateHandle { get; set; }
    }
}
