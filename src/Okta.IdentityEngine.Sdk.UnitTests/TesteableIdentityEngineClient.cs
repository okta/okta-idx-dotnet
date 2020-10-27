using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace Okta.IdentityEngine.Sdk.UnitTests
{
    public class TesteableIdentityEngineClient : OktaIdentityEngineClient
    {
        public static readonly OktaClientConfiguration DefaultFakeConfiguration = new OktaClientConfiguration
        {
            OktaDomain = "https://fake.example.com",
        };

        public TesteableIdentityEngineClient(IRequestExecutor requestExecutor)
            : base(
                new DefaultDataStore(requestExecutor, new DefaultSerializer(), new ResourceFactory(null, null, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource)))),
                    NullLogger.Instance,
                    new UserAgentBuilder("test",
                        typeof(TesteableIdentityEngineClient).GetTypeInfo().Assembly.GetName().Version)),
                DefaultFakeConfiguration,
                new RequestContext())
        {
        }
    }
}
