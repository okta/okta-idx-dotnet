using Microsoft.Extensions.Logging;
using Okta.Idx.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Okta.Idx.Sdk.UnitTests
{
    public class MockIdxClient : IdxClient
    {
        public MockIdxClient(
            IdxConfiguration configuration = null,
            HttpClient httpClient = null,
            ILogger logger = null): base(configuration, httpClient, logger)
        { }

        public int LogErrorCallCount { get; protected set; }

        protected override void LogError(Exception ex)
        {
            ++LogErrorCallCount;
            base.LogError(ex);
        }
    }
}
