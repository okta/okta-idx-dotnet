using Newtonsoft.Json;
using Okta.Idx.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// A descriptor for the current state of an Idx interaction.
    /// </summary>
    public class IdxState
    {
        public string Key
        {
            get => Context.State;
        }

        public IIdxContext Context { get; set; }

        [JsonIgnore]
        public IIdxResponse IntrospectResponse { get; set; }

        [JsonIgnore]
        public IdxClient Client { get; set; }

        public IdxConfiguration Configuration { get; set; }
    }
}
