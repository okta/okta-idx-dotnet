// <copyright file="AuthenticationClient.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Okta.Idx.Sdk.Auth
{
    public class AuthenticationClient
    {
        private IIdxClient _idxClient;

        public AuthenticationClient(IIdxClient idxClient)
        {
            _idxClient = idxClient;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationOptions options)
        {
            var idxContext = await _idxClient.InteractAsync();
            var introspectResponse = await _idxClient.IntrospectAsync(idxContext);

            // Check if identify flow include credentials
            var isIdentifyInOneStep = IsRemediationRequireCredentials("identify", introspectResponse);

            // Common request payload
            var identifyRequest = new IdxRequestPayload();
            identifyRequest.StateHandle = introspectResponse.StateHandle;
            identifyRequest.SetProperty("identifier", options.Username);

            if (isIdentifyInOneStep)
            {
                identifyRequest.SetProperty("credentials", new
                {
                    passcode = options.Password,
                });
            }

            var identifyResponse = await introspectResponse
                                            .Remediation
                                            .RemediationOptions
                                            .FirstOrDefault(x => x.Name == "identify")
                                            .ProceedAsync(identifyRequest);

            if (isIdentifyInOneStep)
            {
                // We expect success
                if (!identifyResponse.IsLoginSuccess)
                {
                    // Verify if password expired
                    if (IsRemediationRequireCredentials("reenroll-authenticator", identifyResponse))
                    {
                        return new AuthenticationResponse
                        {
                            AuthenticationStatus = AuthenticationStatus.PasswordExpired,
                            IdxContext = idxContext,
                        };
                    }
                    else
                    {
                        // TODO: Improve error
                        throw new NotSupportedException("Uknown flow - Review your policies");
                    }
                }

                var tokenResponse = await identifyResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }
            else
            {
                // We expect remediation has credentials now
                if (!IsRemediationRequireCredentials("challenge-authenticator", identifyResponse))
                {
                    // TODO: Improve error
                    throw new NotSupportedException("Uknown flow - Review your policies");
                }

                var challengeRequest = new IdxRequestPayload();
                challengeRequest.StateHandle = identifyResponse.StateHandle;
                challengeRequest.SetProperty("credentials", new
                {
                    passcode = options.Password,
                });

                var challengeResponse = await identifyResponse
                                              .Remediation
                                              .RemediationOptions
                                              .FirstOrDefault(x => x.Name == "challenge-authenticator")
                                              .ProceedAsync(challengeRequest);

                if (!challengeResponse.IsLoginSuccess)
                {
                    // Verify if password expired
                    if (IsRemediationRequireCredentials("reenroll-authenticator", challengeResponse))
                    {
                        return new AuthenticationResponse
                        {
                            AuthenticationStatus = AuthenticationStatus.PasswordExpired,
                            IdxContext = idxContext,
                        };
                    }
                    else
                    {
                        // TODO: Improve error
                        throw new NotSupportedException("Uknown flow - Review your policies");
                    }
                }

                var tokenResponse = await identifyResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }
        }

        public async Task<AuthenticationResponse> ChangePasswordAsync(ChangePasswordOptions changePasswordOptions, IIdxContext idxContext)
        {
            // Re-entry flow with context
            var introspectResponse = await _idxClient.IntrospectAsync(idxContext);

            // Verify if password expired
            if (!IsRemediationRequireCredentials("reenroll-authenticator", introspectResponse))
            {
                // TODO: Improve error
                throw new NotSupportedException("Uknown flow - Review your policies");
            }

            var resetAuthenticatorRequest = new IdxRequestPayload();
            resetAuthenticatorRequest.StateHandle = introspectResponse.StateHandle;
            resetAuthenticatorRequest.SetProperty("credentials", new
            {
                passcode = changePasswordOptions.NewPassword,
            });

            // Reset Password is expected
            var resetPasswordResponse = await introspectResponse
                                              .Remediation
                                              .RemediationOptions
                                              .FirstOrDefault(x => x.Name == "reenroll-authenticator")
                                              .ProceedAsync(resetAuthenticatorRequest);

            if (resetPasswordResponse.IsLoginSuccess)
            {
                var tokenResponse = await resetPasswordResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);

                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Success,
                    TokenInfo = tokenResponse,
                };
            }
            else
            {
                // TODO: Improve error
                throw new NotSupportedException("Uknown flow - Review your policies");
            }
        }

        private static bool IsRemediationRequireCredentials(string remediationOptionName, IIdxResponse idxResponse)
        {
            var jToken = JToken.Parse(idxResponse.GetRaw());

            var credentialsObj = jToken.SelectToken($"$.remediation.value[?(@.name == '{remediationOptionName}')].value[?(@.name == 'credentials')]");

            return credentialsObj != null;
        }

        public class AuthenticationOptions
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }

        public class ChangePasswordOptions
        {
            public string NewPassword { get; set; }
            public string StateHandle { get; set; }
        }

        public class AuthenticationResponse
        {
            // TODO: Create a TokenInfo class
            public ITokenResponse TokenInfo { get; set; }

            public AuthenticationStatus AuthenticationStatus  { get; set; }

            public IIdxContext IdxContext { get; set; }
        }

        public enum AuthenticationStatus
        {
            Success,
            PasswordExpired,
        }
    }
}
