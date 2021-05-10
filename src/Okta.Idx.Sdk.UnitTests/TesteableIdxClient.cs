using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        };

        public TesteableIdxClient(IRequestExecutor requestExecutor)
            : base(
                new DefaultDataStore(requestExecutor, new DefaultSerializer(), new ResourceFactory(null, null, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource)))),
                    NullLogger.Instance,
                    new UserAgentBuilder("test",
                        typeof(TesteableIdxClient).GetTypeInfo().Assembly.GetName().Version)),
                DefaultFakeConfiguration,
                new RequestContext())
        {
        }

        public new async Task<IIdxContext> InteractAsync(string state = null, CancellationToken cancellationToken = default)
        {
            return await base.InteractAsync(state, cancellationToken);
        }

        public new async Task<IIdxResponse> IntrospectAsync(IIdxContext idxContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.IntrospectAsync(idxContext, cancellationToken);
        }
    }
}
