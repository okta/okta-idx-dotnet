using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.IdentityEngine.Sdk.Configuration;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace Okta.IdentityEngine.Sdk.UnitTests
{
    public class TesteableIdentityEngineClient : OktaIdentityEngineClient
    {
        public static readonly OktaIdentityEngineConfiguration DefaultFakeConfiguration = new OktaIdentityEngineConfiguration
        {
            Issuer = "https://fake.example.com",
            ClientId = "foo",
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
