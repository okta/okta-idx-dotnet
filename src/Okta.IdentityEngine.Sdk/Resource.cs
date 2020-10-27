using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Okta.Sdk.Abstractions;

namespace Okta.IdentityEngine.Sdk
{
    /// <summary>
    /// This class represents a resource
    /// </summary>
    public class Resource : BaseResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        public Resource()
        {
            Initialize(null, null, null, null);
        }

        internal void Initialize(
            IOktaIdentityEngineClient client,
            ResourceFactory resourceFactory,
            IDictionary<string, object> data,
            ILogger logger)
        {
            resourceFactory = resourceFactory ?? new ResourceFactory(client, logger, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            base.Initialize(client, resourceFactory, data, logger);
        }

        /// <summary>
        /// Gets the <see cref="IOktaClient">OktaClient</see> that created this resource.
        /// </summary>
        /// <returns>The <see cref="IOktaClient">OktaClient</see> that created this resource.</returns>
        protected new IOktaIdentityEngineClient GetClient()
        {
            return (IOktaIdentityEngineClient)_client ?? throw new InvalidOperationException("Only resources retrieved or saved through a Client object cna call server-side methods.");
        }
    }
}
