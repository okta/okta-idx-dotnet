using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class IdxRequestPayload : Resource
    {
        public string StateHandle
        {
            get => GetStringProperty("stateHandle");
            set => this["stateHandle"] = value;
        }
    }
}
