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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Okta.Idx.Sdk.Configuration;
using Okta.Idx.Sdk.Extensions;
using Okta.Idx.Sdk.Helpers;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration.Providers.EnvironmentVariables;
using Okta.Sdk.Abstractions.Configuration.Providers.Object;
using Okta.Sdk.Abstractions.Configuration.Providers.Yaml;

namespace Okta.Idx.Sdk
{
    /// <summary>
    /// The IDX client.
    /// </summary>
    public class IdxClient : IIdxClient
    {
        private HttpClient httpClient;

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
        /// Gets the Request Options.
        /// </summary>
        public RequestOptions RequestOptions { get; internal set; } = new RequestOptions();

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
            this.httpClient = httpClient ?? DefaultHttpClient.Create(
                connectionTimeout: null,
                proxyConfiguration: null,
                logger: _logger);

            var oktaBaseConfiguration = OktaConfigurationConverter.Convert(Configuration);
            var resourceTypeResolverFactory = new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource)));
            var requestExecutor = new DefaultRequestExecutor(oktaBaseConfiguration, this.httpClient, _logger);
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
            var oktaConfigSection = configBuilder.Build().GetSection("okta");
            oktaConfigSection.GetSection("idx").Bind(compiledConfig);
            oktaConfigSection.GetSection("testing").Bind(compiledConfig);

            compiledConfig.Scopes = compiledConfig.Scopes.Distinct().ToList();
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
        /// Initializes a new instance of the <see cref="IdxClient"/> class.
        /// </summary>
        /// <param name="requestExecutor">The <see cref="IRequestExecutor">Request Executor</see> to use.</param>
        /// <param name="configuration">The client configuration.</param>
        /// <param name="requestContext">The request context, if any.</param>
        /// <remarks>This overload is used internally to create cheap copies of an existing client.</remarks>
        protected IdxClient(IRequestExecutor requestExecutor, IdxConfiguration configuration, RequestContext requestContext)
        {
            Configuration = configuration;

            var userAgentBuilder = new UserAgentBuilder("okta-idx-dotnet", typeof(IdxClient).GetTypeInfo().Assembly.GetName().Version);

            // TODO: Allow proxy configuration
            this.httpClient = httpClient ?? DefaultHttpClient.Create(
                                  connectionTimeout: null,
                                  proxyConfiguration: null,
                                  logger: NullLogger.Instance);

            var oktaBaseConfiguration = OktaConfigurationConverter.Convert(Configuration);
            var resourceTypeResolverFactory = new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource)));
            var resourceFactory = new ResourceFactory(this, NullLogger.Instance, resourceTypeResolverFactory);

            _dataStore = new DefaultDataStore(
                requestExecutor,
                new DefaultSerializer(),
                resourceFactory,
                NullLogger.Instance,
                userAgentBuilder);
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
        /// <param name="activationToken">The activation token. Optional.</param>
        /// <param name="recoveryToken">The recovery token. Optional.</param>
        /// <returns>The IDX context.</returns>
        internal async Task<IIdxContext> InteractAsync(string state = null, CancellationToken cancellationToken = default, string activationToken = null, string recoveryToken = null)
        {
            // PKCE props
            state = state ?? GenerateSecureRandomString(16);
            var codeVerifier = GenerateSecureRandomString(86);
            var codeChallenge = GenerateCodeChallenge(codeVerifier, out var codeChallengeMethod);

            Dictionary<string, string> headers;
            if (RequestOptions?.Headers != null)
            {
                headers = new Dictionary<string, string>(RequestOptions.Headers);
            }
            else
            {
                headers = new Dictionary<string, string>();
            }

            headers.Add(RequestHeaders.ContentType, HttpRequestContentBuilder.ContentTypeFormUrlEncoded);

            var payload = new Dictionary<string, string>
            {
                { "scope", string.Join(" ", Configuration.Scopes) },
                { "client_id", Configuration.ClientId },

                // Add PKCE params and state
                { "code_challenge_method", codeChallengeMethod },
                { "code_challenge", codeChallenge },
                { "redirect_uri", Configuration.RedirectUri },
                { "state", state },
            };

            if (Configuration.IsConfidentialClient)
            {
                payload.Add("client_secret", Configuration.ClientSecret);
                if (!string.IsNullOrEmpty(Configuration.DeviceToken))
                {
                    headers.Add(RequestHeaders.XDeviceToken, Configuration.DeviceToken);
                }
            }

            if (!string.IsNullOrEmpty(activationToken))
            {
                payload.Add("activation_token", activationToken);
            }

            if (!string.IsNullOrEmpty(recoveryToken))
            {
                payload.Add("recovery_token", recoveryToken);
            }

            Uri uri = new Uri(IdxUrlHelper.GetNormalizedUriString(UrlHelper.EnsureTrailingSlash(Configuration.Issuer), "v1/interact"));

            var request = new HttpRequest
            {
                Uri = uri.ToString(),
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
        public async Task<IdentityProvidersResponse> GetIdentityProvidersAsync(string state = null, CancellationToken cancellationToken = default)
        {
            IIdxContext idxContext = await this.InteractAsync(state, cancellationToken);
            return await GetIdentityProvidersAsync(idxContext, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IdentityProvidersResponse> GetIdentityProvidersAsync(IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            IIdxResponse introspectResponse = await this.IntrospectAsync(idxContext, cancellationToken);

            return new IdentityProvidersResponse
            {
                Context = idxContext,
                IdpOptions = introspectResponse.Remediation?.RemediationOptions?
                    .Where(remediationOption => remediationOption.Name.Equals(RemediationType.RedirectIdp))
                    .Select(remediationOption => new IdpOption
                    {
                        State = idxContext.State,
                        InteractionHandle = idxContext.InteractionHandle,
                        Id = remediationOption.Idp.Id,
                        Name = remediationOption.Idp.Name,
                        Href = remediationOption.Href,
                    })
                    .ToList(),
            };
        }

        /// <inheritdoc/>
        public async Task<WidgetSignInResponse> StartWidgetSignInAsync(CancellationToken cancellationToken = default)
        {
            var idxContext = await this.InteractAsync(cancellationToken: cancellationToken);
            return new WidgetSignInResponse
            {
                IdxContext = idxContext,
                SignInWidgetConfiguration = new SignInWidgetConfiguration(this.Configuration, idxContext),
            };
        }

        /// <inheritdoc/>
        public async Task<TokenResponse> RedeemInteractionCodeAsync(IIdxContext idxContext, string interactionCode, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(interactionCode))
                {
                    throw new ArgumentNullException(nameof(interactionCode), "Interaction code was not specified.");
                }

                if (string.IsNullOrEmpty(idxContext.CodeVerifier))
                {
                    throw new ArgumentNullException(nameof(idxContext.CodeVerifier), "CodeVerifier was not specified.");
                }

                Uri issuerUri = new Uri(Configuration.Issuer);
                Uri tokenUri = new Uri(IdxUrlHelper.GetNormalizedUriString(issuerUri.ToString(), "v1/token"));
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenUri);

                StringBuilder requestContent = new StringBuilder();
                IdxUrlHelper.AddParameter(requestContent, "grant_type", "interaction_code", false);
                IdxUrlHelper.AddParameter(requestContent, "client_id", Configuration.ClientId, true);
                if (Configuration.IsConfidentialClient)
                {
                    IdxUrlHelper.AddParameter(requestContent, "client_secret", Configuration.ClientSecret, true);
                }

                IdxUrlHelper.AddParameter(requestContent, "interaction_code", interactionCode, true);
                IdxUrlHelper.AddParameter(requestContent, "code_verifier", idxContext.CodeVerifier, true);

                requestMessage.Content = new StringContent(requestContent.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
                requestMessage.Headers.Add("Accept", "application/json");
                HttpResponseMessage responseMessage = await this.httpClient.SendAsync(requestMessage);
                string tokenResponseJson = await responseMessage.Content.ReadAsStringAsync();

                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new RedeemInteractionCodeException(tokenResponseJson);
                }

                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(tokenResponseJson);
                TokenResponse response = new TokenResponse();
                response.Initialize(this, null, data, _logger);
                return response;
            }
            catch (RedeemInteractionCodeException redeemInteractionCodeException)
            {
                LogError(redeemInteractionCodeException);
                throw;
            }
            catch (Exception exception)
            {
                LogError(exception);
                throw new RedeemInteractionCodeException(exception);
            }
        }

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        protected virtual void LogError(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        /// <inheritdoc/>
        public IOktaClient CreateScoped(RequestContext requestContext)
            => new IdxClient(_dataStore, Configuration, requestContext);

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationOptions authenticationOptions, CancellationToken cancellationToken = default)
        {
            var isPasswordFlow = !string.IsNullOrEmpty(authenticationOptions.Password);

            if (isPasswordFlow)
            {
                return await AuthenticateWithPasswordAsync(authenticationOptions, cancellationToken);
            }

            // assume users will log in with the authenticator they want

            var idxContext = await InteractAsync(cancellationToken: cancellationToken);
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            // Common request payload
            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", authenticationOptions.Username);

            var identifyResponse = await introspectResponse
                                            .Remediation
                                            .RemediationOptions
                                            .FirstOrDefault(x => x.Name == RemediationType.Identify)
                                            .ProceedAsync(identifyRequest, cancellationToken);

            if (identifyResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
            {
                return new AuthenticationResponse
                {
                    IdxContext = idxContext,
                    AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorSelection,
                    Authenticators = IdxResponseHelper.ConvertToAuthenticators(identifyResponse.Authenticators.Value, identifyResponse.AuthenticatorEnrollments.Value),
                };
            }

            if (identifyResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
            {
                return new AuthenticationResponse
                {
                    IdxContext = idxContext,
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                    Authenticators = IdxResponseHelper.ConvertToAuthenticators(identifyResponse.Authenticators.Value),
                };
            }

            throw new UnexpectedRemediationException(
                    new List<string>
                    {
                        RemediationType.SelectAuthenticatorAuthenticate,
                        RemediationType.SelectAuthenticatorEnroll,
                    },
                    introspectResponse);
        }

        private async Task<AuthenticationResponse> AuthenticateWithPasswordAsync(AuthenticationOptions authenticationOptions, CancellationToken cancellationToken = default)
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

            identifyResponse.AssertNotInTerminalState();

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

                    if (identifyResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
                    {
                        return new AuthenticationResponse
                        {
                            IdxContext = idxContext,
                            AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                            Authenticators = IdxResponseHelper.ConvertToAuthenticators(identifyResponse.Authenticators.Value),
                        };
                    }
                    else if (identifyResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
                    {
                        return new AuthenticationResponse
                        {
                            IdxContext = idxContext,
                            AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorSelection,
                            Authenticators = IdxResponseHelper.ConvertToAuthenticators(identifyResponse.Authenticators.Value, identifyResponse.AuthenticatorEnrollments.Value),
                        };
                    }
                    else
                    {
                        throw new UnexpectedRemediationException(
                            new List<string>
                            {
                                RemediationType.ReenrollAuthenticator,
                                RemediationType.SelectAuthenticatorAuthenticate,
                                RemediationType.SelectAuthenticatorEnroll,
                            }, identifyResponse);
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
                if (!identifyResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator)
                    && !identifyResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
                {
                    throw new UnexpectedRemediationException(
                        new List<string>
                        {
                            RemediationType.ChallengeAuthenticator,
                            RemediationType.SelectAuthenticatorAuthenticate,
                        }, identifyResponse);
                }

                var sendPasswordResponse = identifyResponse;

                if (!identifyResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator))
                {
                    var passwordAuthenticator = identifyResponse
                                                 .Authenticators
                                                 .Value
                                                 .FirstOrDefault(x => x.Key == AuthenticatorType.Password.ToIdxKeyString());

                    if (passwordAuthenticator == null)
                    {
                        throw new OktaException("Password is not available for authentication. Please review your policies.");
                    }

                    var selectAuthenticatorRequest = new IdxRequestPayload();
                    selectAuthenticatorRequest.StateHandle = identifyResponse.StateHandle;
                    selectAuthenticatorRequest.SetProperty(
                        "authenticator",
                        new
                        {
                            id = passwordAuthenticator.Id,
                        });

                    var selectAuthenticatorResponse = await identifyResponse
                        .ProceedWithRemediationOptionAsync(RemediationType.SelectAuthenticatorAuthenticate, selectAuthenticatorRequest, cancellationToken);

                    sendPasswordResponse = selectAuthenticatorResponse;
                }

                var challengeRequest = new IdxRequestPayload();
                challengeRequest.StateHandle = identifyResponse.StateHandle;
                challengeRequest.SetProperty("credentials", new
                {
                    passcode = authenticationOptions.Password,
                });

                var challengeResponse = await sendPasswordResponse
                    .ProceedWithRemediationOptionAsync(RemediationType.ChallengeAuthenticator, challengeRequest, cancellationToken);

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

                    if (challengeResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
                    {
                        return new AuthenticationResponse
                        {
                            IdxContext = idxContext,
                            AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                            Authenticators = IdxResponseHelper.ConvertToAuthenticators(challengeResponse.Authenticators.Value),
                        };
                    }
                    else if (challengeResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
                    {
                        return new AuthenticationResponse
                        {
                            IdxContext = idxContext,
                            AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorSelection,
                            Authenticators = IdxResponseHelper.ConvertToAuthenticators(challengeResponse.Authenticators.Value, challengeResponse.AuthenticatorEnrollments.Value),
                        };
                    }
                    else
                    {
                        throw new UnexpectedRemediationException(
                            new List<string>
                            {
                                RemediationType.ReenrollAuthenticator,
                                RemediationType.SelectAuthenticatorEnroll,
                                RemediationType.SelectAuthenticatorAuthenticate,
                            }, challengeResponse);
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
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            var currentRemediationType = RemediationType.Unknown;

            if (introspectResponse.ContainsRemediationOption(RemediationType.ReenrollAuthenticator))
            {
                currentRemediationType = RemediationType.ReenrollAuthenticator;
            }
            else if (introspectResponse.ContainsRemediationOption(RemediationType.ResetAuthenticator))
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

            var resetAuthenticatorRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            resetAuthenticatorRequest.SetProperty("credentials", new
            {
                passcode = changePasswordOptions.NewPassword,
            });

            // Reset password
            var resetPasswordResponse = await introspectResponse
                                              .ProceedWithRemediationOptionAsync(currentRemediationType, resetAuthenticatorRequest, cancellationToken);

            if (resetPasswordResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
            {
                return new AuthenticationResponse
                           {
                               IdxContext = idxContext,
                               AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorSelection,
                               Authenticators = IdxResponseHelper.ConvertToAuthenticators(resetPasswordResponse.Authenticators.Value, resetPasswordResponse.AuthenticatorEnrollments.Value),
                           };
            }

            if (resetPasswordResponse.IsLoginSuccess)
            {
                var tokenResponse = await resetPasswordResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }

            throw new UnexpectedRemediationException(
                new List<string>
                    {
                        RemediationType.SuccessWithInteractionCode,
                        RemediationType.SelectAuthenticatorAuthenticate,
                    },
                resetPasswordResponse);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> SelectChallengeAuthenticatorAsync(SelectPhoneAuthenticatorOptions selectAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var request = new IdxRequestPayload();
            request.SetProperty("authenticator", new
            {
                enrollmentId = selectAuthenticatorOptions.EnrollmentId,
                id = selectAuthenticatorOptions.AuthenticatorId,
            });

            return await SelectChallengeAuthenticatorAsync(request, idxContext, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> SelectChallengeAuthenticatorAsync(SelectAuthenticatorOptions selectAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var request = new IdxRequestPayload();
            request.SetProperty("authenticator", new
            {
                id = selectAuthenticatorOptions.AuthenticatorId,
            });

            return await SelectChallengeAuthenticatorAsync(request, idxContext, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> SkipAuthenticatorSelectionAsync(IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            var skipOption = introspectResponse.FindRemediationOption(RemediationType.Skip, true);
            var skipRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            var skipResponse = await skipOption.ProceedAsync(skipRequest, cancellationToken);

            skipResponse.AssertNotInTerminalState();

            if (skipResponse.IsLoginSuccess)
            {
                var tokenResponse = await skipResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }

            throw new UnexpectedRemediationException(RemediationType.SuccessWithInteractionCode, skipResponse);
        }

        private async Task<AuthenticationResponse> SelectChallengeAuthenticatorAsync(IdxRequestPayload idxRequestPayload, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            if (!introspectResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator)
                && !introspectResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
            {
                throw new UnexpectedRemediationException(
                    new List<string>
                    {
                        RemediationType.ChallengeAuthenticator,
                        RemediationType.SelectAuthenticatorAuthenticate,
                    }, introspectResponse);
            }

            idxRequestPayload.StateHandle = introspectResponse.StateHandle;

            var authenticatorSelectionResponse = await introspectResponse.ProceedWithRemediationOptionAsync(
                RemediationType.SelectAuthenticatorAuthenticate,
                idxRequestPayload,
                cancellationToken);

            // WebAuthN enrollment data is available in authenticatorSelectionResponse.CurrentAuthenticator instead of authenticatorSelectionResponse.CurrentAuthenticatorEnrollment unlike other authenticators
            var currentAuthenticatorEnrollment =
                authenticatorSelectionResponse.CurrentAuthenticatorEnrollment?.Value ??
                authenticatorSelectionResponse.CurrentAuthenticator?.Value;

            if (authenticatorSelectionResponse.ContainsRemediationOption(RemediationType.AuthenticatorVerificationData))
            {
                return new AuthenticationResponse
                {
                    IdxContext = idxContext,
                    AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorData,
                    CurrentAuthenticatorEnrollment = IdxResponseHelper.ConvertToAuthenticator(authenticatorSelectionResponse.Authenticators.Value, currentAuthenticatorEnrollment, authenticatorSelectionResponse.AuthenticatorEnrollments.Value),
                };
            }
            else
            {
               // (authenticatorSelectionResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator))
               return new AuthenticationResponse
                {
                    IdxContext = idxContext,
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorVerification,
                    CurrentAuthenticatorEnrollment = IdxResponseHelper.ConvertToAuthenticator(authenticatorSelectionResponse.Authenticators.Value, currentAuthenticatorEnrollment, authenticatorSelectionResponse.AuthenticatorEnrollments?.Value),
                };
            }
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> ChallengeAuthenticatorAsync(ChallengePhoneAuthenticatorOptions challengeAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var request = new IdxRequestPayload();
            request.SetProperty("authenticator", new
            {
                id = challengeAuthenticatorOptions.AuthenticatorId,
                enrollmentId = challengeAuthenticatorOptions.EnrollmentId,
                methodType = challengeAuthenticatorOptions.MethodType.ToString(),
            });

            return await ChallengeAuthenticatorAsync(request, idxContext, cancellationToken);
        }

        private async Task<AuthenticationResponse> ChallengeAuthenticatorAsync(IdxRequestPayload idxRequestPayload, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            if (!introspectResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator) &&
                !introspectResponse.ContainsRemediationOption(RemediationType.AuthenticatorVerificationData))
            {
                throw new UnexpectedRemediationException(
                    new List<string>
                    {
                        RemediationType.ChallengeAuthenticator,
                        RemediationType.AuthenticatorVerificationData,
                    }, introspectResponse);
            }

            var currentRemediationType = RemediationType.ChallengeAuthenticator;

            if (introspectResponse.ContainsRemediationOption(RemediationType.AuthenticatorVerificationData))
            {
                currentRemediationType = RemediationType.AuthenticatorVerificationData;
            }

            idxRequestPayload.StateHandle = introspectResponse.StateHandle;

            var challengeResponse = await introspectResponse.ProceedWithRemediationOptionAsync(
                                        currentRemediationType,
                                        idxRequestPayload,
                                        cancellationToken);

            if (challengeResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator))
            {
                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorVerification,
                    IdxContext = idxContext,
                };
            }

            throw new UnexpectedRemediationException(RemediationType.ChallengeAuthenticator, challengeResponse);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> RecoverPasswordAsync(RecoverPasswordOptions recoverPasswordOptions, CancellationToken cancellationToken = default)
        {
            var idxContext = await InteractAsync(cancellationToken: cancellationToken, recoveryToken: recoverPasswordOptions.RecoveryToken);
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            if (!string.IsNullOrEmpty(recoverPasswordOptions.RecoveryToken))
            {
                var resetAuthenticatorOption = introspectResponse.FindRemediationOption(RemediationType.ResetAuthenticator);
                if (resetAuthenticatorOption != null)
                {
                    var resetAuthenticatorRequest = new IdxRequestPayload
                    {
                        StateHandle = introspectResponse.StateHandle,
                    };
                    resetAuthenticatorRequest.SetProperty(
                        "credentials",
                        new
                        {
                            passcode = recoverPasswordOptions.Passcode,
                        });

                    var resetResponse = await resetAuthenticatorOption
                        .ProceedAsync(resetAuthenticatorRequest, cancellationToken);

                    if (resetResponse.IsLoginSuccess)
                    {
                        var tokenResponse = await resetResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                        return new AuthenticationResponse
                        {
                            AuthenticationStatus = AuthenticationStatus.Success,
                            TokenInfo = tokenResponse,
                        };
                    }

                    throw new OktaException("Unexpected response. Cannot reset password.");
                }
            }

            var identifyOption = introspectResponse.FindRemediationOption(RemediationType.Identify, throwIfNotFound: true);
            var requiresCredentials = identifyOption.Form.Any(f => f.Name == "credentials");

            IList<IAuthenticator> authenticators;

            if (requiresCredentials && introspectResponse.CurrentAuthenticator.Value != null)
            {
                authenticators = await GetOneStepAuthRecoveryAuthenticators(recoverPasswordOptions, introspectResponse, cancellationToken);
            }
            else
            {
                authenticators = await GetTwoStepAuthRecoveryAuthenticators(recoverPasswordOptions, introspectResponse, cancellationToken);
            }

            return new AuthenticationResponse
            {
                IdxContext = idxContext,
                AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorSelection,
                Authenticators = authenticators,
            };
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> ResendCodeAsync(IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            var resendRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            if (introspectResponse?.CurrentAuthenticatorEnrollment != null
                && introspectResponse?.CurrentAuthenticatorEnrollment.Value?.Resend != null)
            {
                var resendResponse = await introspectResponse.CurrentAuthenticatorEnrollment.Value.Resend.ProceedAsync(resendRequest, cancellationToken);
                if (!resendResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator))
                {
                    throw new UnexpectedRemediationException(RemediationType.ChallengeAuthenticator, resendResponse);
                }

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorVerification,
                    IdxContext = idxContext,
                    CurrentAuthenticatorEnrollment = IdxResponseHelper.ConvertToAuthenticator(resendResponse.Authenticators.Value, resendResponse.CurrentAuthenticatorEnrollment.Value, resendResponse.AuthenticatorEnrollments.Value),
                };
            }
            else if (introspectResponse?.CurrentAuthenticator != null
                     && introspectResponse?.CurrentAuthenticator.Value?.Resend != null)
            {
                var resendResponse = await introspectResponse.CurrentAuthenticator.Value.Resend.ProceedAsync(resendRequest, cancellationToken);

                if (!resendResponse.ContainsRemediationOption(RemediationType.EnrollAuthenticator))
                {
                    throw new UnexpectedRemediationException(RemediationType.EnrollAuthenticator, resendResponse);
                }

                return new AuthenticationResponse
                {
                    IdxContext = idxContext,
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorVerification,
                    CurrentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(resendResponse.Authenticators.Value, resendResponse.CurrentAuthenticator.Value, resendResponse.AuthenticatorEnrollments.Value),
                };
            }
            else
            {
                throw new UnexpectedRemediationException(new List<string> { RemediationType.ChallengeAuthenticator, RemediationType.EnrollAuthenticator }, introspectResponse);
            }
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> SelectRecoveryAuthenticatorAsync(SelectAuthenticatorOptions selectAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            IIdxResponse recoveryResponse = null;

            if (introspectResponse.CurrentAuthenticatorEnrollment != null)
            {
                var recoveryRequest = new IdxRequestPayload
                {
                    StateHandle = introspectResponse.StateHandle,
                };

                var currentEnrollment = introspectResponse
                                            .CurrentAuthenticatorEnrollment
                                            .Value;

                recoveryResponse = await currentEnrollment
                                            .Recover
                                            .ProceedAsync(recoveryRequest, cancellationToken);

                var recoveryAuthenticator = recoveryResponse?
                    .Authenticators
                    .Value
                    .FirstOrDefault(x => x.Id == selectAuthenticatorOptions.AuthenticatorId);

                if (recoveryAuthenticator == null)
                {
                    throw new OktaException($"Authenticator not found. Verify that you have the selected authenticator enabled for your application.");
                }
            }
            else
            {
                if (introspectResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
                {
                    recoveryResponse = introspectResponse;
                }
                else
                {
                    throw new UnexpectedRemediationException(new List<string> { RemediationType.SelectAuthenticatorAuthenticate }, introspectResponse);
                }
            }

            var selectAuthenticatorRequest = new IdxRequestPayload
            {
                StateHandle = recoveryResponse.StateHandle,
            };

            selectAuthenticatorRequest.SetProperty("authenticator", new
            {
                id = selectAuthenticatorOptions.AuthenticatorId,
            });

            var selectRecoveryAuthenticatorRemediationOption = await recoveryResponse
                    .ProceedWithRemediationOptionAsync(RemediationType.SelectAuthenticatorAuthenticate, selectAuthenticatorRequest, cancellationToken);

            if (!selectRecoveryAuthenticatorRemediationOption.ContainsRemediationOption(RemediationType.ChallengeAuthenticator))
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
        public async Task<AuthenticationResponse> EnrollAuthenticatorAsync(
            EnrollWebAuthnAuthenticatorOptions verifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            var currentRemediationType = RemediationType.Unknown;

            if (introspectResponse.ContainsRemediationOption(RemediationType.EnrollAuthenticator))
            {
                currentRemediationType = RemediationType.EnrollAuthenticator;
            }
            else
            {
                throw new UnexpectedRemediationException(RemediationType.EnrollAuthenticator, introspectResponse);
            }

            var verifyAuthenticatorRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            verifyAuthenticatorRequest.SetProperty("credentials", new
            {
                attestation = verifyAuthenticatorOptions.Attestation,
                clientData = verifyAuthenticatorOptions.ClientData,
            });

            var enrollAuthenticatorResponse = await introspectResponse
                .ProceedWithRemediationOptionAsync(currentRemediationType, verifyAuthenticatorRequest, cancellationToken);

            var isAuthenticatorEnroll = enrollAuthenticatorResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll);

            if (enrollAuthenticatorResponse.IsLoginSuccess)
            {
                var tokenResponse = await enrollAuthenticatorResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }

            if (isAuthenticatorEnroll)
            {
                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                    Authenticators = IdxResponseHelper.ConvertToAuthenticators(enrollAuthenticatorResponse.Authenticators.Value),
                    IdxContext = idxContext,
                    CanSkip = enrollAuthenticatorResponse.ContainsRemediationOption(RemediationType.Skip),
                };
            }

            throw new UnexpectedRemediationException(
                new List<string>
                {
                    RemediationType.SelectAuthenticatorEnroll,
                },
                enrollAuthenticatorResponse);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> VerifyAuthenticatorAsync(OktaVerifyVerifyAuthenticatorOptions verifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var challengeAuthenticatorRequest = new IdxRequestPayload();
            challengeAuthenticatorRequest.SetProperty("credentials", new
            {
                totp = verifyAuthenticatorOptions.TotpCode,
            });

            return await VerifyAuthenticatorAsync(challengeAuthenticatorRequest, idxContext, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorOptions verifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var challengeAuthenticatorRequest = new IdxRequestPayload();
            challengeAuthenticatorRequest.SetProperty("credentials", new
            {
                passcode = verifyAuthenticatorOptions.Code,
            });

            return await VerifyAuthenticatorAsync(challengeAuthenticatorRequest, idxContext, cancellationToken);
        }

        private async Task<AuthenticationResponse> VerifyAuthenticatorAsync(IdxRequestPayload challengeAuthenticatorRequest, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            var currentRemediationType = RemediationType.Unknown;

            // Check if flow is challenge authenticator or enroll authenticator, otherwise throw
            if (introspectResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator))
            {
                currentRemediationType = RemediationType.ChallengeAuthenticator;
            }
            else if (introspectResponse.ContainsRemediationOption(RemediationType.EnrollAuthenticator))
            {
                currentRemediationType = RemediationType.EnrollAuthenticator;
            }
            else if (introspectResponse.ContainsRemediationOption(RemediationType.EnrollPoll))
            {
                currentRemediationType = RemediationType.EnrollPoll;
            }
            else if (introspectResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
            {
                currentRemediationType = RemediationType.SelectAuthenticatorEnroll;
            }
            else
            {
                if (currentRemediationType == RemediationType.EnrollAuthenticator &&
                    !IsRemediationRequireCredentials(RemediationType.EnrollAuthenticator, introspectResponse))
                {
                    throw new UnexpectedRemediationException(RemediationType.EnrollAuthenticator, introspectResponse);
                }
                else if (currentRemediationType == RemediationType.ChallengeAuthenticator &&
                    !IsRemediationRequireCredentials(RemediationType.ChallengeAuthenticator, introspectResponse))
                {
                    throw new UnexpectedRemediationException(RemediationType.ChallengeAuthenticator, introspectResponse);
                }
                else
                {
                    throw new UnexpectedRemediationException(
                        new List<string>
                        {
                            RemediationType.ChallengeAuthenticator,
                            RemediationType.EnrollAuthenticator,
                            RemediationType.SelectAuthenticatorEnroll,
                            RemediationType.EnrollPoll,
                        },
                        introspectResponse);
                }
            }

            challengeAuthenticatorRequest.StateHandle = introspectResponse.StateHandle;

            return await VerifyAuthenticatorAsync(challengeAuthenticatorRequest, introspectResponse, currentRemediationType, idxContext, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> ChallengeAuthenticatorAsync(ChallengeWebAuthnAuthenticatorOptions verifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            var currentRemediationType = RemediationType.Unknown;

            // Check if flow is challenge authenticator or enroll authenticator, otherwise throw
            if (introspectResponse.ContainsRemediationOption(RemediationType.ChallengeAuthenticator))
            {
                currentRemediationType = RemediationType.ChallengeAuthenticator;
            }
            else
            {
                throw new UnexpectedRemediationException(RemediationType.ChallengeAuthenticator, introspectResponse);
            }

            var challengeAuthenticatorRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            challengeAuthenticatorRequest.SetProperty("credentials", new
            {
                authenticatorData = verifyAuthenticatorOptions.AuthenticatorData,
                clientData = verifyAuthenticatorOptions.ClientData,
                signatureData = verifyAuthenticatorOptions.SignatureData,
            });

            return await VerifyAuthenticatorAsync(challengeAuthenticatorRequest, introspectResponse, currentRemediationType, idxContext, cancellationToken);
        }

        private async Task<AuthenticationResponse> VerifyAuthenticatorAsync(IdxRequestPayload payload, IIdxResponse proceedResponse, string remediationType, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var challengeAuthenticatorResponse = await proceedResponse
                .ProceedWithRemediationOptionAsync(remediationType, payload, cancellationToken);

            var isResetAuthenticator = challengeAuthenticatorResponse.ContainsRemediationOption(RemediationType.ResetAuthenticator);
            var isAuthenticatorEnroll = challengeAuthenticatorResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll);

            if (challengeAuthenticatorResponse.IsLoginSuccess)
            {
                var tokenResponse = await challengeAuthenticatorResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }

            if (isResetAuthenticator)
            {
                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingPasswordReset,
                    IdxContext = idxContext,
                };
            }

            if (isAuthenticatorEnroll)
            {
                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                    Authenticators = IdxResponseHelper.ConvertToAuthenticators(challengeAuthenticatorResponse.Authenticators.Value),
                    IdxContext = idxContext,
                    CanSkip = challengeAuthenticatorResponse.ContainsRemediationOption(RemediationType.Skip),
                };
            }

            throw new UnexpectedRemediationException(
                      new List<string>
                      {
                            RemediationType.ResetAuthenticator,
                            RemediationType.SelectAuthenticatorEnroll,
                      },
                      challengeAuthenticatorResponse);
        }

        /// <inheritdoc/>
        public async Task RevokeTokensAsync(TokenType tokenType, string token, CancellationToken cancellationToken = default)
        {
            var payload = new Dictionary<string, string>
            {
                { "client_id", Configuration.ClientId },
            };

            var headers = new Dictionary<string, string>
            {
                { RequestHeaders.ContentType, HttpRequestContentBuilder.ContentTypeFormUrlEncoded },
            };

            if (Configuration.IsConfidentialClient)
            {
                payload.Add("client_secret", Configuration.ClientSecret);
            }

            payload.Add("token_type_hint", tokenType.ToTokenHintString());
            payload.Add("token", token);

            Uri uri = new Uri(IdxUrlHelper.GetNormalizedUriString(UrlHelper.EnsureTrailingSlash(Configuration.Issuer), "v1/revoke"));
            var request = new HttpRequest
            {
                Uri = uri.ToString(),
                Payload = payload,
                Headers = headers,
            };

            await PostAsync<Resource>(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> RegisterAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
        {
            var idxContext = await InteractAsync(cancellationToken: cancellationToken);
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            var enrollRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            // choose enroll option
            var enrollProfileResponse = await introspectResponse.ProceedWithRemediationOptionAsync(RemediationType.SelectEnrollProfile, enrollRequest, cancellationToken);

            var enrollNewProfileRequest = new IdxRequestPayload
            {
                StateHandle = enrollProfileResponse.StateHandle,
            };
            enrollNewProfileRequest.SetProperty("userProfile", userProfile);

            var enrollNewProfileResponse = await enrollProfileResponse.ProceedWithRemediationOptionAsync(RemediationType.EnrollProfile, enrollNewProfileRequest, cancellationToken);

            if (!enrollNewProfileResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
            {
                throw new UnexpectedRemediationException(RemediationType.SelectAuthenticatorEnroll, enrollNewProfileResponse);
            }

            return new AuthenticationResponse
            {
                IdxContext = idxContext,
                AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                Authenticators = IdxResponseHelper.ConvertToAuthenticators(enrollNewProfileResponse.Authenticators.Value),
                CanSkip = enrollNewProfileResponse.ContainsRemediationOption(RemediationType.Skip),
            };
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> SelectEnrollAuthenticatorAsync(SelectEnrollAuthenticatorOptions enrollAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var selectAuthenticatorRequest = new IdxRequestPayload();
            selectAuthenticatorRequest.SetProperty("authenticator", new
            {
                id = enrollAuthenticatorOptions.AuthenticatorId,
            });

            return await EnrollAuthenticatorAsync(selectAuthenticatorRequest, idxContext, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> EnrollAuthenticatorAsync(EnrollPhoneAuthenticatorOptions enrollAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var selectAuthenticatorRequest = new IdxRequestPayload();
            selectAuthenticatorRequest.SetProperty("authenticator", new
            {
                id = enrollAuthenticatorOptions.AuthenticatorId,
                methodType = enrollAuthenticatorOptions.MethodType.ToString().ToLower(),
                phoneNumber = enrollAuthenticatorOptions.PhoneNumber,
            });

            return await EnrollAuthenticatorAsync(selectAuthenticatorRequest, idxContext, cancellationToken);
        }

        private async Task<AuthenticationResponse> EnrollAuthenticatorAsync(IdxRequestPayload selectAuthenticatorRequest, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            // Re-entry flow with context
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            selectAuthenticatorRequest.StateHandle = introspectResponse.StateHandle;

            var selectAuthenticatorEnrollRemediationOption = introspectResponse
                                                             .Remediation
                                                             .RemediationOptions
                                                             .FirstOrDefault(x => x.Name == RemediationType.SelectAuthenticatorEnroll);

            // Verify if enroll is required
            if (selectAuthenticatorEnrollRemediationOption == null)
            {
                throw new UnexpectedRemediationException(RemediationType.SelectAuthenticatorEnroll, introspectResponse);
            }

            var selectAuthenticatorResponse = await selectAuthenticatorEnrollRemediationOption.ProceedAsync(selectAuthenticatorRequest, cancellationToken);
            var currentRemediationType = RemediationType.Unknown;
            var status = AuthenticationStatus.AwaitingAuthenticatorVerification;

            // Check if flow is challenge authenticator or enroll authenticator, otherwise throw
            if (selectAuthenticatorResponse.Remediation.RemediationOptions.Any(x => x.Name == RemediationType.AuthenticatorEnrollmentData))
            {
                currentRemediationType = RemediationType.AuthenticatorEnrollmentData;
                status = AuthenticationStatus.AwaitingAuthenticatorEnrollmentData;
            }
            else if (selectAuthenticatorResponse.Remediation.RemediationOptions.Any(x => x.Name == RemediationType.EnrollAuthenticator))
            {
                currentRemediationType = RemediationType.EnrollAuthenticator;
            }
            else if (selectAuthenticatorResponse.ContainsRemediationOption(RemediationType.EnrollPoll))
            {
                currentRemediationType = RemediationType.EnrollPoll;
            }
            else if (selectAuthenticatorResponse.ContainsRemediationOption(RemediationType.SelectEnrollmentChannel))
            {
                currentRemediationType = RemediationType.SelectEnrollmentChannel;
            }

            if (currentRemediationType != RemediationType.EnrollPoll &&
            currentRemediationType != RemediationType.EnrollAuthenticator &&
            currentRemediationType != RemediationType.AuthenticatorEnrollmentData &&
            currentRemediationType != RemediationType.SelectEnrollmentChannel)
            {
                throw new UnexpectedRemediationException(
                    new List<string>
                    {
                            RemediationType.EnrollPoll,
                            RemediationType.AuthenticatorEnrollmentData,
                            RemediationType.EnrollAuthenticator,
                            RemediationType.SelectEnrollmentChannel,
                    },
                    selectAuthenticatorResponse);
            }

            var currentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(
                selectAuthenticatorResponse.Authenticators.Value,
                selectAuthenticatorResponse.CurrentAuthenticator.Value,
                selectAuthenticatorResponse.AuthenticatorEnrollments.Value);

            // Assuming Okta Verify for now
            if (currentRemediationType == RemediationType.EnrollPoll ||
                currentRemediationType == RemediationType.SelectEnrollmentChannel)
            {
                // TODO: Move this to convertToAuthenticator
                currentAuthenticator.ChannelTypes = selectAuthenticatorResponse.FindRemediationOption(RemediationType.SelectEnrollmentChannel)
                    .Form?
                    .FirstOrDefault(x => x.Name == "authenticator")?.GetProperty<IFormValue>("value")?
                    .Form?
                    .GetArrayProperty<IFormValue>("value")?
                    .FirstOrDefault(x => x.Name == "channel")?.Options
                    ?.Select(x => x.GetProperty<AuthenticatorChannelType>("value")).ToList();
            }

            return new AuthenticationResponse
            {
                IdxContext = idxContext,
                AuthenticationStatus = status,
                CurrentAuthenticator = currentAuthenticator,
            };
        }

        private static bool IsRemediationRequireCredentials(string remediationOptionName, IIdxResponse idxResponse)
        {
            var jwtToken = JToken.Parse(idxResponse.GetRaw());

            var credentialsObj = jwtToken.SelectToken($"$.remediation.value[?(@.name == '{remediationOptionName}')].value[?(@.name == 'credentials')]");

            return credentialsObj != null;
        }

        private async Task<IList<IAuthenticator>> GetOneStepAuthRecoveryAuthenticators(RecoverPasswordOptions recoverPasswordOptions, IIdxResponse introspectResponse, CancellationToken cancellationToken)
        {
            // Recovery request first
            var recoveryRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            var currentAuthenticator = introspectResponse
                            .CurrentAuthenticator
                            .Value;

            var recoveryResponse = await currentAuthenticator
                                        .Recover
                                        .ProceedAsync(recoveryRequest, cancellationToken);

            // Get available authenticators
            var identifyRequest = new IdxRequestPayload
            {
                StateHandle = recoveryResponse.StateHandle,
            };
            identifyRequest.SetProperty("identifier", recoverPasswordOptions.Username);

            var identifyResponse = await recoveryResponse.ProceedWithRemediationOptionAsync(RemediationType.IdentifyRecovery, identifyRequest, cancellationToken);

            identifyResponse.AssertNotInTerminalState();

            var recoveryAuthenticators = identifyResponse.Authenticators.Value;

            return IdxResponseHelper.ConvertToAuthenticators(recoveryAuthenticators);
        }

        private async Task<IList<IAuthenticator>> GetTwoStepAuthRecoveryAuthenticators(RecoverPasswordOptions recoverPasswordOptions, IIdxResponse introspectResponse, CancellationToken cancellationToken)
        {
            // Common request payload
            var identifyRequest = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };
            identifyRequest.SetProperty("identifier", recoverPasswordOptions.Username);

            // Send username
            var identifyResponse = await introspectResponse.ProceedWithRemediationOptionAsync(RemediationType.Identify, identifyRequest, cancellationToken);

            identifyResponse.AssertNotInTerminalState();

            // Get available authenticators
            var recoveryRequest = new IdxRequestPayload
            {
                StateHandle = identifyResponse.StateHandle,
            };

            var currentEnrollment = identifyResponse
                            .CurrentAuthenticatorEnrollment
                            .Value;

            var recoveryResponse = await currentEnrollment
                                        .Recover
                                        .ProceedAsync(recoveryRequest, cancellationToken);

            var recoveryAuthenticators = recoveryResponse.Authenticators.Value;

            return IdxResponseHelper.ConvertToAuthenticators(recoveryAuthenticators);
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

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> SelectChallengeAuthenticatorAsync(SelectOktaVerifyAuthenticatorOptions challengeAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            if (!introspectResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
            {
                throw new UnexpectedRemediationException(
                    new List<string>
                    {
                        RemediationType.SelectAuthenticatorAuthenticate,
                    }, introspectResponse);
            }

            var idxRequestPayload = new IdxRequestPayload();
            idxRequestPayload.SetProperty("authenticator", new { id = challengeAuthenticatorOptions.AuthenticatorId, methodType = challengeAuthenticatorOptions.AuthenticatorMethodType });
            idxRequestPayload.SetProperty("stateHandle", introspectResponse.StateHandle);

            var selectAuthenticatorResponse = await introspectResponse.ProceedWithRemediationOptionAsync(
                RemediationType.SelectAuthenticatorAuthenticate,
                idxRequestPayload,
                cancellationToken);

            var authenticationResponse = new AuthenticationResponse
            {
                IdxContext = idxContext,
                AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorVerification,
                CurrentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(selectAuthenticatorResponse.Authenticators.Value, selectAuthenticatorResponse.CurrentAuthenticator.Value),
                CanSkip = selectAuthenticatorResponse.ContainsRemediationOption(RemediationType.Skip),
            };

            return authenticationResponse;
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> SelectEnrollAuthenticatorAsync(EnrollOktaVerifyAuthenticatorOptions enrollAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            if (!introspectResponse.ContainsRemediationOption(RemediationType.SelectEnrollmentChannel))
            {
                throw new UnexpectedRemediationException(
                    new List<string>
                    {
                        RemediationType.SelectEnrollmentChannel,
                    }, introspectResponse);
            }

            var idxRequestPayload = new IdxRequestPayload();
            idxRequestPayload.SetProperty("authenticator", new { channel = enrollAuthenticatorOptions.Channel, id = enrollAuthenticatorOptions.AuthenticatorId });
            idxRequestPayload.SetProperty("stateHandle", introspectResponse.StateHandle);

            var selectEnrollmentChannelResponse = await introspectResponse.ProceedWithRemediationOptionAsync(
                RemediationType.SelectEnrollmentChannel, idxRequestPayload, cancellationToken);
            var remediationOption = selectEnrollmentChannelResponse.FindRemediationOption(RemediationType.SelectEnrollmentChannel, true);
            var currentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(
                selectEnrollmentChannelResponse.Authenticators?.Value,
                selectEnrollmentChannelResponse.CurrentAuthenticator?.Value);

            // TODO: Move this to convertToAuthenticator
            currentAuthenticator.ChannelTypes = remediationOption.Form?.FirstOrDefault(x => x.Name == "channel")?.Options
                ?.Select(x => x.GetProperty<AuthenticatorChannelType>("value")).ToList();

            var authenticationResponse = new AuthenticationResponse
            {
                IdxContext = idxContext,
                AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollmentData,
                CurrentAuthenticator = currentAuthenticator,
                CanSkip = selectEnrollmentChannelResponse.ContainsRemediationOption(RemediationType.Skip),
            };

            return authenticationResponse;
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> EnrollAuthenticatorAsync(EnrollOktaVerifyAuthenticatorOptions oktaVerifyAuthenticatorOptions, IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            if (!introspectResponse.ContainsRemediationOption(RemediationType.EnrollmentChannelData))
            {
                throw new UnexpectedRemediationException(
                    new List<string>
                    {
                        RemediationType.EnrollmentChannelData,
                    }, introspectResponse);
            }

            var idxRequestPayload = new IdxRequestPayload { StateHandle = introspectResponse.StateHandle };

            switch (oktaVerifyAuthenticatorOptions.Channel)
            {
                case "email":
                    idxRequestPayload.SetProperty("email", oktaVerifyAuthenticatorOptions.Email);
                    break;
                case "sms":
                    idxRequestPayload.SetProperty("phoneNumber", oktaVerifyAuthenticatorOptions.PhoneNumber);
                    break;
            }

            var enrollmentChannelDataResponse = await introspectResponse.ProceedWithRemediationOptionAsync(RemediationType.EnrollmentChannelData, idxRequestPayload, cancellationToken);

            var authenticationResponse = new AuthenticationResponse
            {
                IdxContext = idxContext,
                AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                CurrentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(enrollmentChannelDataResponse.Authenticators?.Value, enrollmentChannelDataResponse.CurrentAuthenticator?.Value),
                CanSkip = enrollmentChannelDataResponse.ContainsRemediationOption(RemediationType.Skip),
            };

            return authenticationResponse;
        }

        /// <inheritdoc/>
        public async Task<PollResponse> PollAuthenticatorEnrollmentStatusAsync(IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);
            bool continuePolling = introspectResponse.ContainsRemediationOption(RemediationType.EnrollPoll, out IRemediationOption remediationOption);

            if (continuePolling)
            {
                return new PollResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                    Refresh = remediationOption?.Refresh,
                    ContinuePolling = continuePolling,
                    CurrentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(introspectResponse.Authenticators?.Value, introspectResponse.CurrentAuthenticator?.Value),
                    CanSkip = introspectResponse.ContainsRemediationOption(RemediationType.Skip),
                };
            }
            else
            {
                if (introspectResponse.SuccessWithInteractionCode != null)
                {
                    return new PollResponse
                    {
                        AuthenticationStatus = AuthenticationStatus.Success,
                        TokenInfo = await introspectResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext),
                        CanSkip = introspectResponse.ContainsRemediationOption(RemediationType.Skip),
                    };
                }

                if (introspectResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
                {
                    return new PollResponse
                    {
                        IdxContext = idxContext,
                        AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorSelection,
                        Authenticators = IdxResponseHelper.ConvertToAuthenticators(introspectResponse.Authenticators?.Value, introspectResponse.AuthenticatorEnrollments?.Value),
                        CanSkip = introspectResponse.ContainsRemediationOption(RemediationType.Skip),
                    };
                }

                if (introspectResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
                {
                    return new PollResponse
                    {
                        IdxContext = idxContext,
                        AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                        Authenticators = IdxResponseHelper.ConvertToAuthenticators(introspectResponse.Authenticators?.Value),
                        CanSkip = introspectResponse.ContainsRemediationOption(RemediationType.Skip),
                    };
                }
            }

            throw new UnexpectedRemediationException(
                new List<string>
                {
                    RemediationType.EnrollPoll,
                    RemediationType.SelectAuthenticatorAuthenticate,
                    RemediationType.SelectAuthenticatorEnroll,
                },
                introspectResponse);
        }

        /// <inheritdoc/>
        public async Task<PollResponse> PollAuthenticatorPushStatusAsync(IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            var introspectResponse = await IntrospectAsync(idxContext, cancellationToken);

            IdxRequestPayload requestPayload = new IdxRequestPayload
            {
                StateHandle = introspectResponse.StateHandle,
            };

            if (!introspectResponse.ContainsRemediationOption(RemediationType.ChallengePoll))
            {
                throw new UnexpectedRemediationException(RemediationType.ChallengePoll, introspectResponse);
            }

            var challengeResponse = await introspectResponse.ProceedWithRemediationOptionAsync(RemediationType.ChallengePoll, requestPayload, cancellationToken);
            bool continuePolling = challengeResponse.ContainsRemediationOption(RemediationType.ChallengePoll, out IRemediationOption challengePollRemediationOption);

            if (continuePolling)
            {
                return new PollResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorVerification,
                    ContinuePolling = continuePolling,
                    Refresh = challengePollRemediationOption?.Refresh,
                };
            }

            if (challengeResponse.SuccessWithInteractionCode != null)
            {
                var tokenInfo = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext, cancellationToken);
                return new PollResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenInfo,
                    ContinuePolling = continuePolling,
                    Refresh = challengePollRemediationOption?.Refresh,
                };
            }

            if (challengeResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
            {
                return new PollResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorSelection,
                    ContinuePolling = continuePolling,
                    Refresh = challengePollRemediationOption?.Refresh,
                };
            }

            if (challengeResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
            {
                return new PollResponse
                {
                    AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                    ContinuePolling = continuePolling,
                    Refresh = challengePollRemediationOption?.Refresh,
                };
            }

            throw new UnexpectedRemediationException(
                new List<string>
                {
                    RemediationType.SelectAuthenticatorAuthenticate,
                    RemediationType.SelectAuthenticatorEnroll,
                },
                challengeResponse);
        }

        /// <inheritdoc/>
        public async Task<PasswordRequiredResponse> CheckIsPasswordRequiredAsync(string state = null, CancellationToken cancellationToken = default)
        {
            var idxContext = await this.InteractAsync(state, cancellationToken);
            return await CheckIsPasswordRequiredAsync(idxContext, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<PasswordRequiredResponse> CheckIsPasswordRequiredAsync(IIdxContext idxContext, CancellationToken cancellationToken = default)
        {
            if (idxContext == null)
            {
                throw new ArgumentNullException(nameof(idxContext));
            }

            var introspectResponse = await this.IntrospectAsync(idxContext, cancellationToken);

            return new PasswordRequiredResponse
            {
                Context = idxContext,
                IsPasswordRequired = IsRemediationRequireCredentials("identify", introspectResponse),
            };
        }
    }
}
