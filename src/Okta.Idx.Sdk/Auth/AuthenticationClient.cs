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

        public async Task<ITokenResponse> AuthenticateAsync(AuthenticationOptions options)
        {
            var idxContext = await _idxClient.InteractAsync();
            var introspectResponse = await _idxClient.IntrospectAsync(idxContext);

            // Check if identify flow include credentials
            var isIdentifyInOneStep = IdentifyRequireCredentials("identify", introspectResponse);

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
                    // TODO: Improve error
                    throw new NotSupportedException("Uknown flow - Review your policies");
                }

                return await identifyResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
            }
            else
            {
                // We expect remediation has credentials now
                if (!IdentifyRequireCredentials("challenge-authenticator", identifyResponse))
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

                // We expect success
                if (!challengeResponse.IsLoginSuccess)
                {
                    // TODO: Improve error
                    throw new NotSupportedException("Uknown flow - Review your policies");
                }

                return await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(idxContext);
            }
        }


        private static bool IdentifyRequireCredentials(string remediationOptionName, IIdxResponse idxResponse)
        {
            var jToken = JToken.Parse(idxResponse.GetRaw());

            var credentialsObj = jToken.SelectToken($"$.remediation.value[?(@.name == '{remediationOptionName}')].value[?(@.name == 'credentials')]");


            return credentialsObj != null;
        }

        private static bool HasProperty(JToken token, string propertyName)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var prop in token.Children<JProperty>())
                {
                    if (prop.Name == propertyName)
                    {
                        return true;
                    }

                    var child = prop.Value;

                    if (child != null && child.HasValues)
                    {
                        child = HasProperty(child, propertyName);
                    }
                }

                return false;
            }

            return false;
        }

        public class AuthenticationOptions
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }
    }
}
