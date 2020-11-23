using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Okta.Idx.Sdk
{
    public class IdxResponse : Resource, IIdxResponse
    {
        public string StateHandle => GetStringProperty("stateHandle");

        public string Version => GetStringProperty("version");

        public DateTimeOffset? ExpiresAt => GetDateTimeProperty("expiresAt");

        public string Intent => GetStringProperty("intent");

        public IRemediation Remediation => GetResourceProperty<Remediation>("remediation");

        public string Raw => GetStringProperty("raw");

        public bool LoginSuccess {
            get
            {
                return this.GetData().ContainsKey("successWithInteractionCode");
            }
        }

        public IIdxSuccessResponse SuccessWithInteractionCode => GetResourceProperty<IdxSuccessResponse>("successWithInteractionCode");

        public Task<IIdxResponse> CancelAsync()
        {
            return null;
        }
    }
}
