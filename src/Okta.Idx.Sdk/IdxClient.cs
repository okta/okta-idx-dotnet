// <copyright file="IdxClient.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlexibleConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
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

        /// <summary>
        /// Builds and gets the IDX configuration.
        /// </summary>
        /// <param name="configuration">The IDX configuration.</param>
        /// <returns>The built configuration</returns>
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

        /// <summary>
        /// Calls the Idx interact endpoint to get an IDX context.
        /// </summary>
        /// <param name="state">Optional value to use as the state argument when initiating the authentication flow. This is used to provide contextual information to survive redirects.</param>
        /// <param name="cancellationToken">The cancellation token. Optional.</param>
        /// <returns>The IDX context.</returns>
        internal async Task<IIdxContext> InteractAsync(string state = null, CancellationToken cancellationToken = default)
        {
            // PKCE props
            state = state ?? GenerateSecureRandomString(16);
            var codeVerifier = GenerateSecureRandomString(86);
            var codeChallenge = GenerateCodeChallenge(codeVerifier, out var codeChallengeMethod);

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

            return new IdxContext(codeVerifier, codeChallenge, codeChallengeMethod, response.InteractionHandle, state);
        }

        /// <summary>
        /// Calls the Idx introspect endpoint to get remediation steps.
        /// </summary>
        /// <param name="idxContext">The IDX context that was returned by the `interact()` call</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The IdxResponse.</returns>
        internal async Task<IIdxResponse> IntrospectAsync(IIdxContext idxContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payload = new IdxRequestPayload();
            payload.SetProperty("interactionHandle", idxContext.InteractionHandle);

            var oktaDomain = UrlHelper.GetOktaRootUrl(this.Configuration.Issuer);

            var uri = $"{UrlHelper.EnsureTrailingSlash(oktaDomain)}idp/idx/introspect";

            var headers = new Dictionary<string, string>
            {
                { "Accept", "application/ion+json; okta-version=1.0.0" },
            };

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

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationOptions authenticationOptions, CancellationToken cancellationToken = default)
        {
            var idxContext = await InteractAsync(cancellationToken: cancellationToken);
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            // Check if identify flow include credentials
            var isIdentifyInOneStep = IsRemediationRequireCredentials(RemediationType.Identify, introspectResponse);

            // Common request payload
            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", authenticationOptions.Username);

            if (isIdentifyInOneStep)
            {
                identifyRequest.SetProperty("credentials", new
                {
                    passcode = authenticationOptions.Password,
                });
            }

            var identifyResponse = await introspectResponse
                                            .Remediation
                                            .RemediationOptions
                                            .FirstOrDefault(x => x.Name == RemediationType.Identify)
                                            .ProceedAsync(identifyRequest, cancellationToken);

            if (isIdentifyInOneStep)
            {
                // We expect success
                if (!identifyResponse.IsLoginSuccess)
                {
                    // Verify if password expired
                    if (IsRemediationRequireCredentials(RemediationType.ReenrollAuthenticator, identifyResponse))
                    {
                        return new AuthenticationResponse
                        {
                            AuthenticationStatus = AuthenticationStatus.PasswordExpired,
                            IdxContext = idxContext,
                        };
                    }
                    else
                    {
                        throw new UnexpectedRemediationException(RemediationType.ReenrollAuthenticator, identifyResponse);
                    }
                }

                var tokenResponse = await identifyResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }
            else
            {
                // We expect remediation has credentials now
                if (!IsRemediationRequireCredentials(RemediationType.ChallengeAuthenticator, identifyResponse))
                {
                    throw new UnexpectedRemediationException(RemediationType.ChallengeAuthenticator, identifyResponse);
                }

                var challengeRequest = new IdxRequestPayload();
                challengeRequest.StateHandle = identifyResponse.StateHandle;
                challengeRequest.SetProperty("credentials", new
                {
                    passcode = authenticationOptions.Password,
                });

                var challengeResponse = await identifyResponse
                                              .Remediation
                                              .RemediationOptions
                                              .FirstOrDefault(x => x.Name == RemediationType.ChallengeAuthenticator)
                                              .ProceedAsync(challengeRequest, cancellationToken);

                if (!challengeResponse.IsLoginSuccess)
                {
                    // Verify if password expired
                    if (IsRemediationRequireCredentials(RemediationType.ReenrollAuthenticator, challengeResponse))
                    {
                        return new AuthenticationResponse
                        {
                            AuthenticationStatus = AuthenticationStatus.PasswordExpired,
                            IdxContext = idxContext,
                        };
                    }
                    else
                    {
                        throw new UnexpectedRemediationException(RemediationType.ReenrollAuthenticator, challengeResponse);
                    }
                }

                var tokenResponse = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> ChangePasswordAsync(ChangePasswordOptions changePasswordOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext);
            var currentRemediationType = RemediationType.Unknown;

            // Check if flow is password expiration or forgot password, otherwise throw
            if (introspectResponse.Remediation.RemediationOptions.Any(x => x.Name == RemediationType.ReenrollAuthenticator))
            {
                currentRemediationType = RemediationType.ReenrollAuthenticator;
            }
            else if (introspectResponse.Remediation.RemediationOptions.Any(x => x.Name == RemediationType.ResetAuthenticator))
            {
                currentRemediationType = RemediationType.ResetAuthenticator;
            }
            else
            {
                if (currentRemediationType == RemediationType.ReenrollAuthenticator &&
                    !IsRemediationRequireCredentials(RemediationType.ReenrollAuthenticator, introspectResponse))
                {
                    throw new UnexpectedRemediationException(RemediationType.ReenrollAuthenticator, introspectResponse);
                }
                else
                {
                    throw new UnexpectedRemediationException(
                        new List<string>
                        {
                            RemediationType.ReenrollAuthenticator,
                            RemediationType.ResetAuthenticator,
                        },
                        introspectResponse);
                }
            }

            var resetAuthenticatorRequest = new IdxRequestPayload();
            resetAuthenticatorRequest.StateHandle = introspectResponse.StateHandle;
            resetAuthenticatorRequest.SetProperty("credentials", new
            {
                passcode = changePasswordOptions.NewPassword,
            });

            // Reset password
            var resetPasswordResponse = await introspectResponse
                                              .Remediation
                                              .RemediationOptions
                                              .FirstOrDefault(x => x.Name == currentRemediationType)
                                              .ProceedAsync(resetAuthenticatorRequest, cancellationToken);

            if (resetPasswordResponse.IsLoginSuccess)
            {
                var tokenResponse = await resetPasswordResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }
            else
            {
                throw new UnexpectedRemediationException(RemediationType.SuccessWithInteractionCode, resetPasswordResponse);
            }
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> RecoverPasswordAsync(RecoverPasswordOptions recoverPasswordOptions, CancellationToken cancellationToken = default) 
        {
            var idxContext = await InteractAsync(cancellationToken: cancellationToken);
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            // Common request payload
            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", recoverPasswordOptions.Username);

            // Send username
            var identifyResponse = await introspectResponse
                                        .Remediation
                                        .RemediationOptions
                                        .FirstOrDefault(x => x.Name == RemediationType.Identify)
                                        .ProceedAsync(identifyRequest, cancellationToken);

            // Proceed with recovery
            var recoveryRequest = new IdxRequestPayload();
            recoveryRequest.StateHandle = identifyResponse.StateHandle;

            var recoveryResponse = await identifyResponse
                                        .CurrentAuthenticatorEnrollment
                                        .Value
                                        .Recover
                                        .ProceedAsync(recoveryRequest, cancellationToken);

            var recoveryAuthenticator = recoveryResponse
               .Authenticators
               .Value
               .Where(x => x.Key == recoverPasswordOptions.AuthenticatorType.ToIdxKeyString())
               .FirstOrDefault();

            if (recoveryAuthenticator == null)
            {
                throw new OktaException($"Authenticator not found. Verify that you have {Enum.GetName(typeof(AuthenticatorType), recoverPasswordOptions.AuthenticatorType)} enabled for your app.");
            }

            // Send code
            var selectAuthenticatorRequest = new IdxRequestPayload();
            selectAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
            selectAuthenticatorRequest.SetProperty("authenticator", new
            {
                id = recoveryAuthenticator.Id,
            });

            var selectRecoveryAuthenticatorRemediationOption = await recoveryResponse
                                                            .Remediation
                                                            .RemediationOptions
                                                            .FirstOrDefault(x => x.Name == RemediationType.SelectAuthenticatorAuthenticate)
                                                            .ProceedAsync(selectAuthenticatorRequest, cancellationToken);

            if (!selectRecoveryAuthenticatorRemediationOption
                .Remediation.RemediationOptions
                .Any(x => x.Name == RemediationType.ChallengeAuthenticator))
            {
                throw new UnexpectedRemediationException(RemediationType.ChallengeAuthenticator, selectRecoveryAuthenticatorRemediationOption);
            }

            return new AuthenticationResponse
            {
                AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorVerification,
                IdxContext = idxContext,
            };
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorOptions verifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext);

            // Verify if password expired
            if (!IsRemediationRequireCredentials(RemediationType.ChallengeAuthenticator, introspectResponse))
            {
                throw new UnexpectedRemediationException(RemediationType.ChallengeAuthenticator, introspectResponse);
            }

            var challengeAuthenticatorRequest = new IdxRequestPayload();
            challengeAuthenticatorRequest.StateHandle = introspectResponse.StateHandle;
            challengeAuthenticatorRequest.SetProperty("credentials", new
            {
                passcode = verifyAuthenticatorOptions.Code,
            });

            var challengeAuthenticatorResponse = await introspectResponse
                                                .Remediation.RemediationOptions
                                                .FirstOrDefault(x => x.Name == RemediationType.ChallengeAuthenticator)
                                                .ProceedAsync(challengeAuthenticatorRequest, cancellationToken);

            if (!challengeAuthenticatorResponse.Remediation.RemediationOptions.Any(x => x.Name == RemediationType.ResetAuthenticator))
            {
                throw new UnexpectedRemediationException(RemediationType.ResetAuthenticator, challengeAuthenticatorResponse);
            }

            return new AuthenticationResponse
            {
                AuthenticationStatus = AuthenticationStatus.AwaitingPasswordReset,
                IdxContext = idxContext,
            };
        }

        private static bool IsRemediationRequireCredentials(string remediationOptionName, IIdxResponse idxResponse)
        {
            var jToken = JToken.Parse(idxResponse.GetRaw());

            var credentialsObj = jToken.SelectToken($"$.remediation.value[?(@.name == '{remediationOptionName}')].value[?(@.name == 'credentials')]");

            return credentialsObj != null;
        }

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
