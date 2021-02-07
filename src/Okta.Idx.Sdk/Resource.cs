// <copyright file="Resource.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
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

        internal new void Initialize(
            IOktaClient client,
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
        protected new IIdxClient GetClient()
        {
            return (IIdxClient)_client ?? throw new InvalidOperationException("Only resources retrieved or saved through a Client object can call server-side methods.");
        }
    }
}
