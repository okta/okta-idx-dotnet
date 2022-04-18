using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace Okta.Idx.Sdk.UnitTests
{
    public class TesteableIdxClient : IdxClient
    {
        public static readonly IdxConfiguration DefaultFakeConfiguration = new IdxConfiguration
        {
            Issuer = "https://fake.example.com",
            ClientId = "foo",
            RedirectUri = "https://fake.example.com/redirectUri",
            //ClientSecret = "bar"
        };
        public TesteableIdxClient(IRequestExecutor requestExecutor, DeviceContext deviceContext = null)
            : base(
                requestExecutor,
                DefaultFakeConfiguration,
                deviceContext)
        {
        }

        public TesteableIdxClient(IRequestExecutor requestExecutor, IdxConfiguration configuration, DeviceContext deviceContext = null)
            : base(
        requestExecutor,
        configuration,
        deviceContext) {}
    }
}
