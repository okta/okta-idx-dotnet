using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class InteractionHandleResponse : Resource, IInteractionHandleResponse
    {
        public string InteractionHandle => GetStringProperty("interaction_handle");
    }
}
