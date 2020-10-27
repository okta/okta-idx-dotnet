using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.IdentityEngine.Sdk
{
    public class IdentityEngineRequest : Resource
    {
        public string StateHandle
        {
            get => GetStringProperty("stateHandle");
            set => this["stateHandle"] = value;
        }
    }
}
