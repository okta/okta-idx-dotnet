using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.IdentityEngine.Sdk
{
    public class Remediation : Resource, IRemediation
    {
        public string Type => GetStringProperty("type");

        public IList<IRemediationOption> RemediationOptions => GetArrayProperty<IRemediationOption>("value");
    }
}
