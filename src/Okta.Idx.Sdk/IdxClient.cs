// <copyright file="IdxClient.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlexibleConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Idx.Sdk.Configuration;
using Okta.Sdk.Abstractions;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// The IDX client.
    /// </summary>
    public class IdxClient : IIdxClient
    {
        /// <summary>
        /// The <code>IDataStore</code> implementation to be used for making requests.
        /// </summary>
        private IDataStore _dataStore;

        /// <summary>
        /// The request context to be used when making requests.
        /// </summary>
        private RequestContext _requestContext;

        private ILogger _logger;

        static IdxClient()
        {
            System.AppContext.SetSwitch("Switch.System.Net.DontEnableSystemDefaultTlsVersions", false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdxClient"/> class.
        /// </summary>
        public IdxClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdxClient"/> class using the specified <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="configuration">
        /// The client configuration. If <c>null</c>, the library will attempt to load
        /// configuration from an <c>okta.yaml</c> file or environment variables.
        /// </param>
        /// <param name="httpClient">The HTTP client to use for requests to the Okta API.</param>
        /// <param name="logger">The logging interface to use, if any.</param>
        public IdxClient(
            IdxConfiguration configuration = null,
            HttpClient httpClient = null,
            ILogger logger = null)
        {
            Configuration = GetConfigurationOrDefault(configuration);
            IdxConfigurationValidator.Validate(Configuration);
            _logger = logger ?? NullLogger.Instance;

            var userAgentBuilder = new UserAgentBuilder("okta-idx-dotnet", typeof(IdxClient).GetTypeInfo().Assembly.GetName().Version);

            // TODO: Allow proxy configuration
            httpClient = httpClient ?? DefaultHttpClient.Create(
                connectionTimeout: null,
                proxyConfiguration: null,
                logger: _logger);

            var oktaBaseConfiguration = OktaConfigurationConverter.Convert(Configuration);
            var resourceTypeResolverFactory = new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource)));
            var requestExecutor = new DefaultRequestExecutor(oktaBaseConfiguration, httpClient, _logger);
            var resourceFactory = new ResourceFactory(this, _logger, resourceTypeResolverFactory);

            _dataStore = new DefaultDataStore(
                requestExecutor,
                new DefaultSerializer(),
                resourceFactory,
                _logger,
                userAgentBuilder);
        }

        protected static IdxConfiguration GetConfigurationOrDefault(IdxConfiguration configuration)
        {
            string configurationFileRoot = Directory.GetCurrentDirectory();

            var homeOktaYamlLocation = HomePath.Resolve("~", ".okta", "okta.yaml");

            var applicationAppSettingsLocation = Path.Combine(configurationFileRoot ?? string.Empty, "appsettings.json");
            var applicationOktaYamlLocation = Path.Combine(configurationFileRoot ?? string.Empty, "okta.yaml");

            var configBuilder = new ConfigurationBuilder()
                .AddYamlFile(homeOktaYamlLocation, optional: true)
                .AddJsonFile(applicationAppSettingsLocation, optional: true)
                .AddYamlFile(applicationOktaYamlLocation, optional: true)
                .AddEnvironmentVariables("okta", "_", root: "okta")
                .AddEnvironmentVariables("okta_testing", "_", root: "okta")
                .AddObject(configuration, root: "okta:idx")
                .AddObject(configuration, root: "okta:testing")
                .AddObject(configuration);

            var compiledConfig = new IdxConfiguration();
            configBuilder.Build().GetSection("okta").GetSection("idx").Bind(compiledConfig);
            configBuilder.Build().GetSection("okta").GetSection("testing").Bind(compiledConfig);

            return compiledConfig;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdxClient"/> class.
        /// </summary>
        /// <param name="dataStore">The <see cref="IDataStore">DataStore</see> to use.</param>
        /// <param name="configuration">The client configuration.</param>
        /// <param name="requestContext">The request context, if any.</param>
        /// <remarks>This overload is used internally to create cheap copies of an existing client.</remarks>
        protected IdxClient(IDataStore dataStore, IdxConfiguration configuration, RequestContext requestContext)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            Configuration = configuration;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Gets or sets the Okta configuration.
        /// </summary>
        public IdxConfiguration Configuration { get; protected set; }

        private string GenerateSecureRandomString(int byteCount)
        {
            using (RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[byteCount];
                randomNumberGenerator.GetBytes(data);

                return UrlFormatter.EncodeToBase64Url(data);
            }
        }

        /// <summary>
        /// Generates the <see cref="CodeChallenge"/> for use in PKCE
        /// </summary>
        private string GenerateCodeChallenge(string codeVerifier, out string codeChallengeMethod)
        {
            codeChallengeMethod = "S256";

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

                return UrlFormatter.EncodeToBase64Url(bytes);
            }
        }

        /// <inheritdoc/>
        public async Task<IIdxContext> InteractAsync(CancellationToken cancellationToken = default)
        {
            // PKCE props
            var state = GenerateSecureRandomString(16);
            var codeVerifier = GenerateSecureRandomString(86);
            var codeChallengeMethod = string.Empty;
            var codeChallenge = GenerateCodeChallenge(codeVerifier, out codeChallengeMethod);

            var payload = new Dictionary<string, string>();
            payload.Add("scope", string.Join(" ", Configuration.Scopes));
            payload.Add("client_id", Configuration.ClientId);

            // Add PKCE params and state
            payload.Add("code_challenge_method", codeChallengeMethod);
            payload.Add("code_challenge", codeChallenge);
            payload.Add("redirect_uri", Configuration.RedirectUri);
            payload.Add("state", state);

            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", HttpRequestContentBuilder.ContentTypeFormUrlEncoded);

            var request = new HttpRequest
            {
                Uri = $"{UrlHelper.EnsureTrailingSlash(Configuration.Issuer)}v1/interact",
                Payload = payload,
                Headers = headers,
            };

            var response = await PostAsync<InteractionHandleResponse>(
                request, cancellationToken).ConfigureAwait(false);

            return new IdxContext(codeVerifier, codeChallenge, codeChallengeMethod, response.InteractionHandle);
        }

        /// <inheritdoc/>
        public async Task<IIdxResponse> IntrospectAsync(string interactionHandle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(interactionHandle))
            {
                var interactResponse = await InteractAsync(cancellationToken);

                interactionHandle = interactResponse.InteractionHandle;
            }

            var payload = new IdxRequestPayload();
            payload.SetProperty("interactionHandle", interactionHandle);

            var oktaDomain = UrlHelper.GetOktaRootUrl(this.Configuration.Issuer);

            var uri = $"{UrlHelper.EnsureTrailingSlash(oktaDomain)}idp/idx/introspect";

            var headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/ion+json; okta-version=1.0.0");

            var request = new HttpRequest
            {
                Uri = uri,
                Payload = payload,
                Headers = headers,
            };

            return await PostAsync<IdxResponse>(
                request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public IOktaClient CreateScoped(RequestContext requestContext)
            => new IdxClient(_dataStore, Configuration, requestContext);

        /// <summary>
        /// Creates a new <see cref="CollectionClient{T}"/> given an initial HTTP request.
        /// </summary>
        /// <typeparam name="T">The collection client item type.</typeparam>
        /// <param name="initialRequest">The initial HTTP request.</param>
        /// <returns>The collection client.</returns>
        protected CollectionClient<T> GetCollectionClient<T>(HttpRequest initialRequest)
            where T : IResource
            => new CollectionClient<T>(_dataStore, initialRequest, _requestContext);

        /// <inheritdoc/>
        public Task<T> GetAsync<T>(string href, CancellationToken cancellationToken = default(CancellationToken))
            where T : BaseResource, new()
            => GetAsync<T>(new HttpRequest { Uri = href }, cancellationToken);

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            where T : BaseResource, new()
        {
            var response = await _dataStore.GetAsync<T>(request, _requestContext, cancellationToken).ConfigureAwait(false);
            return response?.Payload;
        }

        /// <inheritdoc/>
        public CollectionClient<T> GetCollection<T>(string href)
            where T : IResource
            => GetCollection<T>(new HttpRequest
            {
                Uri = href,
            });

        /// <inheritdoc/>
        public CollectionClient<T> GetCollection<T>(HttpRequest request)
            where T : IResource
            => GetCollectionClient<T>(request);

        /// <inheritdoc/>
        public Task PostAsync(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            => PostAsync(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task<TResponse> PostAsync<TResponse>(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : BaseResource, new()
            => PostAsync<TResponse>(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task PostAsync(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            => PostAsync<BaseResource>(request, cancellationToken);

        /// <inheritdoc/>
        public async Task<TResponse> PostAsync<TResponse>(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : BaseResource, new()
        {
            var response = await _dataStore.PostAsync<TResponse>(request, _requestContext, cancellationToken).ConfigureAwait(false);
            return response?.Payload;
        }

        /// <inheritdoc/>
        public Task PutAsync(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            => PutAsync(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task<TResponse> PutAsync<TResponse>(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : BaseResource, new()
            => PutAsync<TResponse>(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task PutAsync(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            => PutAsync<BaseResource>(request, cancellationToken);

        /// <inheritdoc/>
        public async Task<TResponse> PutAsync<TResponse>(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : BaseResource, new()
        {
            var response = await _dataStore.PutAsync<TResponse>(request, _requestContext, cancellationToken).ConfigureAwait(false);
            return response?.Payload;
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string href, CancellationToken cancellationToken = default(CancellationToken))
            => DeleteAsync(new HttpRequest { Uri = href }, cancellationToken);

        /// <inheritdoc/>
        public Task DeleteAsync(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            => _dataStore.DeleteAsync(request, _requestContext, cancellationToken);

        /// <inheritdoc/>
        public async Task<TResponse> SendAsync<TResponse>(HttpRequest request, HttpVerb httpVerb, CancellationToken cancellationToken = default)
            where TResponse : BaseResource, new()
        {
            switch (httpVerb)
            {
                case HttpVerb.Get:
                    return await GetAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
                case HttpVerb.Post:
                    return await PostAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
                case HttpVerb.Put:
                    return await PutAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
                case HttpVerb.Delete:
                    await DeleteAsync(request, cancellationToken).ConfigureAwait(false);
                    return null;
                default:
                    return await GetAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
