using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk.IntegrationTests
{
    public class TestIdxClient
    {
        public static IdxClient Create(IdxConfiguration configuration = null)
        {
            // Configuration is expected to be stored in environment variables on the test machine.
            // A few tests pass in a configuration object, but this is just to override and test specific things.
            return new IdxClient(configuration);
        }
    }
}
