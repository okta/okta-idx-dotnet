// <copyright file="OktaVerifyAuthenticationOptions.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Okta.Idx.Sdk.Helpers;

namespace Okta.Idx.Sdk.OktaVerify
{
    /// <summary>
    /// A class that contains the options for authentication with Okta Verify.
    /// </summary>
    public class OktaVerifyAuthenticationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OktaVerifyAuthenticationOptions"/> class.
        /// </summary>
        /// <param name="authenticationResponse">The authentication response.</param>
        /// <param name="idxResponse">The Idx response.</param>
        public OktaVerifyAuthenticationOptions(AuthenticationResponse authenticationResponse, IIdxResponse idxResponse)
        {
            this.AuthenticationResponse = authenticationResponse;
            this.IdxContext = authenticationResponse.IdxContext;
            this.StateHandle = idxResponse.StateHandle;
            this.SelectAuthenticatorAuthenticateRemediationOption = idxResponse.FindRemediationOption(RemediationType.SelectAuthenticatorAuthenticate);
            this.CurrentAuthenticator = idxResponse.GetProperty<IIdxResponse>("currentAuthenticator").GetProperty<AuthenticatorValue>("value");
        }

        /// <summary>
        /// Gets or sets the authentication response.
        /// </summary>
        protected internal AuthenticationResponse AuthenticationResponse { get; set; }

        /// <summary>
        /// Gets the Idx context.
        /// </summary>
        protected IIdxContext IdxContext { get; }

        /// <summary>
        /// Gets or sets the state handle.
        /// </summary>
        public string StateHandle { get; set; }

        /// <summary>
        /// Gets the `select-authenticator-authenticate` remediation option.
        /// </summary>
        protected IRemediationOption SelectAuthenticatorAuthenticateRemediationOption { get; }

        /// <summary>
        /// Gets the `challenge-poll` remediation option.
        /// </summary>
        protected IRemediationOption ChallengePollRemediationOption { get; private set; }

        /// <summary>
        /// Gets the `challenge-authenticator` remediation option.
        /// </summary>
        protected IRemediationOption ChallengeAuthenticatorRemedationOption { get; private set; }

        /// <summary>
        /// Gets the current authenticator.
        /// </summary>
        protected internal IAuthenticatorValue CurrentAuthenticator { get; }

        /// <summary>
        /// Selects the specified authentication method type.
        /// </summary>
        /// <param name="methodType">The method type.</param>
        /// <returns>The authentication response.</returns>
        protected async Task<AuthenticationResponse> SelectOktaVerifyMethodAsync(string methodType)
        {
            IdxRequestPayload idxRequestPayload = new IdxRequestPayload();
            idxRequestPayload.SetProperty("authenticator", new { id = this.CurrentAuthenticator.Id, methodType = methodType });
            idxRequestPayload.SetProperty("stateHandle", StateHandle);

            var selectAuthenticatorResponse = await SelectAuthenticatorAuthenticateRemediationOption.ProceedAsync(idxRequestPayload);
            this.ChallengePollRemediationOption = selectAuthenticatorResponse.FindRemediationOption(RemediationType.ChallengePoll);
            this.ChallengeAuthenticatorRemedationOption = selectAuthenticatorResponse.FindRemediationOption(RemediationType.ChallengeAuthenticator);

            var authenticationResponse = new AuthenticationResponse
            {
                IdxContext = IdxContext,
                AuthenticationStatus = AuthenticationResponse.AuthenticationStatus,
                CurrentAuthenticator = IdxResponseHelper.ConvertToAuthenticator(selectAuthenticatorResponse.Authenticators.Value, selectAuthenticatorResponse.CurrentAuthenticator.Value),
            };

            return authenticationResponse;
        }

        /// <summary>
        /// Gets the token response.
        /// </summary>
        public ITokenResponse TokenInfo { get; internal set; }

        /// <summary>
        /// Selects the TOTP authentication method.
        /// </summary>
        /// <returns>The authentication response.</returns>
        public async Task<AuthenticationResponse> SelectTotpMethodAsync()
        {
            return await SelectOktaVerifyMethodAsync("totp");
        }

        /// <summary>
        /// Selects the push authentication method.
        /// </summary>
        /// <returns>The authentication response.</returns>
        public async Task<AuthenticationResponse> SelectPushMethodAsync()
        {
            return await SelectOktaVerifyMethodAsync("push");
        }

        /// <summary>
        /// Submits the specified TOTP code for authentication.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The authentication response.</returns>
        /// <exception cref="ArgumentException">If the challenge authenticator is not valid.</exception>
        public async Task<AuthenticationResponse> EnterCodeAsync(string code)
        {
            if (ChallengeAuthenticatorRemedationOption.Name != RemediationType.ChallengeAuthenticator)
            {
                throw new ArgumentException($"Expected remediation option of type '{RemediationType.ChallengeAuthenticator}', the specified remediation option is of type {ChallengeAuthenticatorRemedationOption.Name}.");
            }

            IdxRequestPayload requestPayload = new IdxRequestPayload
            {
                StateHandle = StateHandle,
            };
            requestPayload.SetProperty("credentials", new { totp = code });
            try
            {
                var challengeResponse = await ChallengeAuthenticatorRemedationOption.ProceedAsync(requestPayload);
                if (challengeResponse.SuccessWithInteractionCode != null)
                {
                    TokenInfo = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(IdxContext);
                    return new AuthenticationResponse
                    {
                        AuthenticationStatus = AuthenticationStatus.Success,
                        TokenInfo = TokenInfo,
                    };
                }

                if (challengeResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorAuthenticate))
                {
                    return new AuthenticationResponse
                    {
                        IdxContext = IdxContext,
                        AuthenticationStatus = AuthenticationStatus.AwaitingChallengeAuthenticatorSelection,
                        Authenticators = IdxResponseHelper.ConvertToAuthenticators(challengeResponse.Authenticators.Value, challengeResponse.AuthenticatorEnrollments.Value),
                    };
                }

                if (challengeResponse.ContainsRemediationOption(RemediationType.SelectAuthenticatorEnroll))
                {
                    return new AuthenticationResponse
                    {
                        IdxContext = IdxContext,
                        AuthenticationStatus = AuthenticationStatus.AwaitingAuthenticatorEnrollment,
                        Authenticators = IdxResponseHelper.ConvertToAuthenticators(challengeResponse.Authenticators.Value),
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
            catch (Exception ex)
            {
                return new AuthenticationResponse
                {
                    AuthenticationStatus = AuthenticationStatus.Exception,
                    MessageToUser = ex.Message,
                };
            }
        }

        /// <summary>
        /// Sends a request to the challenge polling endpoint.
        /// </summary>
        /// <returns>OktaVerifyPollResponse.</returns>
        /// <exception cref="ArgumentException">If the challenge authenticator is not valid.</exception>
        public async Task<OktaVerifyPollResponse> PollOnceAsync()
        {
            if (ChallengePollRemediationOption.Name != RemediationType.ChallengePoll)
            {
                throw new ArgumentException($"Expected remediation option of type '{RemediationType.ChallengePoll}', the specified remediation option is of type {ChallengePollRemediationOption.Name}.");
            }

            IdxRequestPayload requestPayload = new IdxRequestPayload
            {
                StateHandle = StateHandle,
            };

            var challengeResponse = await ChallengePollRemediationOption.ProceedAsync(requestPayload);
            bool continuePolling = challengeResponse.ContainsRemediationOption(RemediationType.ChallengePoll, out IRemediationOption challengePollRemediationOption);
            if (continuePolling == false)
            {
                this.TokenInfo = await challengeResponse.SuccessWithInteractionCode.ExchangeCodeAsync(IdxContext);
            }

            return new OktaVerifyPollResponse
            {
                Refresh = challengePollRemediationOption?.Refresh,
                ContinuePolling = continuePolling,
            };
        }
    }
}
