using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Okta.IdentityEngine.Sdk
{
    public class OktaIdentityEngineResponse : Resource, IOktaIdentityEngineResponse
    {
        public string StateHandle => GetStringProperty("stateHandle");

        public string Version => GetStringProperty("version");

        public DateTimeOffset? ExpiresAt => GetDateTimeProperty("expiresAt");

        public string Intent => GetStringProperty("intent");

        public IRemediation Remediation => GetResourceProperty<Remediation>("remediation");

        public string Raw => GetStringProperty("raw");

        public Task<IOktaIdentityEngineResponse> Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
