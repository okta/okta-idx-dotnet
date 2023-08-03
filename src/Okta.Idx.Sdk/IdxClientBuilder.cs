// <copyright file="IdxClientBuilder.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// Component used to build an IdxClient.
    /// </summary>
    public class IdxClientBuilder
    {
        private ServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdxClientBuilder"/> class.
        /// </summary>
        public IdxClientBuilder()
        {
            this._services = new ServiceCollection();
            this.UseConfiguration(IdxClient.GetConfigurationOrDefault(null));
            this.UseLogger(NullLogger.Instance);
            this.UseUserAgentBuilder(new UserAgentBuilder("okta-idx-dotnet", typeof(IdxClient).GetTypeInfo().Assembly.GetName().Version));

            this.UseHttpClient(DefaultHttpClient.Create(
                connectionTimeout: null,
                proxyConfiguration: null,
                logger: NullLogger.Instance));

            this.UsePasswordWarnStateResolver(PasswordWarnStateResolver.Default);
        }

        /// <summary>
        /// Use the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>IdxClientBuilder.</returns>
        public IdxClientBuilder UseConfiguration(IdxConfiguration configuration)
        {
            this.Remove<IdxConfiguration>();
            this.Remove<OktaClientConfiguration>();
            this._services.AddSingleton(configuration);
            this._services.AddSingleton(svcProvider => OktaConfigurationConverter.Convert(svcProvider.GetRequiredService<IdxConfiguration>()));

            return this;
        }

        /// <summary>
        /// Use the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>IdxClientBuilder.</returns>
        public IdxClientBuilder UseLogger(ILogger logger)
        {
            this.Remove<ILogger>();
            this._services.AddSingleton(logger);
            return this;
        }

        /// <summary>
        /// Use the specified UserAgentBuilder.
        /// </summary>
        /// <param name="userAgentBuilder">The UserAgentBuilder.</param>
        /// <returns>IdxClientBuilder.</returns>
        public IdxClientBuilder UseUserAgentBuilder(UserAgentBuilder userAgentBuilder)
        {
            this.Remove<UserAgentBuilder>();
            this._services.AddSingleton(userAgentBuilder);
            return this;
        }

        /// <summary>
        /// Use the specified HttpClient.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>IdxClientBuilder.</returns>
        public IdxClientBuilder UseHttpClient(HttpClient client)
        {
            this.Remove<HttpClient>();
            this._services.AddSingleton(client);
            return this;
        }

        /// <summary>
        /// Use the specified function to determine when the pipeline is in a password warning state.
        /// </summary>
        /// <param name="checkPasswordWarnState">The implementation.</param>
        /// <returns>IdxClientBuilder.</returns>
        public IdxClientBuilder UsePasswordWarnStateResolver(Func<IIdxResponse, bool> checkPasswordWarnState)
        {
            return UsePasswordWarnStateResolver(new CustomPasswordWarnStateResolver(checkPasswordWarnState));
        }

        /// <summary>
        /// Use the specified implementation to determine when the pipeline is in a password warning state.
        /// </summary>
        /// <param name="passwordWarnStateResolver">The IPasswordWarnStateResolver implementation.</param>
        /// <returns>IdxClientBuilder.</returns>
        public IdxClientBuilder UsePasswordWarnStateResolver(IPasswordWarnStateResolver passwordWarnStateResolver)
        {
            this.Remove<IPasswordWarnStateResolver>();
            this._services.AddSingleton(passwordWarnStateResolver);
            return this;
        }

        /// <summary>
        /// Build the IdxClient.
        /// </summary>
        /// <param name="validate">A value indicating whether to validate the configuration.</param>
        /// <returns>IdxClient.</returns>
        public IdxClient Build(bool validate = false)
        {
            IdxClient client = new IdxClient(false);
            client.Initialize(GetServiceCollection(client));
            if (validate)
            {
                ValidateConfigurationOrDie(client);
            }

            return client;
        }

        /// <summary>
        /// Throws an ArgumentException if the configuration is not valid.
        /// </summary>
        /// <param name="client">The client whose configuration is checked.</param>
        public void ValidateConfigurationOrDie(IdxClient client)
        {
            IdxConfigurationValidator.Validate(client.Configuration);
        }

        /// <summary>
        /// Get a service collection using the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>ServiceCollection.</returns>
        protected ServiceCollection GetServiceCollection(IdxClient client)
        {
            this._services.AddSingleton((svcProvider) => new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));

            this._services.AddSingleton<IRequestExecutor>((svcProvider) => new DefaultRequestExecutor(svcProvider.GetRequiredService<OktaClientConfiguration>(), svcProvider.GetRequiredService<HttpClient>(), svcProvider.GetService<ILogger>()));

            this._services.AddSingleton((serviceProvider) => new ResourceFactory(client, serviceProvider.GetService<ILogger>(), serviceProvider.GetService<AbstractResourceTypeResolverFactory>()));

            this._services.AddSingleton<ISerializer>((svcProvider) => new DefaultSerializer());
            this._services.AddSingleton<IDataStore>((svcProvider) => new DefaultDataStore(svcProvider.GetRequiredService<IRequestExecutor>(), svcProvider.GetRequiredService<ISerializer>(), svcProvider.GetRequiredService<ResourceFactory>(), svcProvider.GetRequiredService<ILogger>(), svcProvider.GetRequiredService<UserAgentBuilder>()));

            return this._services;
        }

        private void Remove<T>()
        {
            _services.Remove(_services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T)));
        }
    }
}
